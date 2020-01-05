using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TacoLib.Interop;

namespace TacoLib.Data
{
    public class AldlIoMessageParser : ITacoMessageParser
    {
        private readonly TacoConfiguration _config;
        private readonly ILogger<AldlIoMessageParser> _logger;

        public AldlIoMessageParser(TacoConfiguration config, ILogger<AldlIoMessageParser> logger)
        {
            _config = config;
            _logger = logger;
        }

        public bool TryParseMessage(byte[] buffer, int length, TacoDefinitions definitions, out TacoMessage message)
        {
            using var ms = new MemoryStream(buffer, 0, length);
            return TryParseMessage(ms, definitions, out message);
        }

        public bool TryParseMessage(Stream s, TacoDefinitions definitions, out TacoMessage message)
        {
            message = default;
            var r = new BinaryReader(s, Encoding.ASCII, true);
            var t = r.ReadInt64();
            var count = r.ReadInt32();
            var dataSize = r.ReadInt32();
            if (count != _config.DataCount || t < 0 || _config.DataSize != dataSize)
            {
                _logger.LogInformation(
                    $"Found invalid data: time {t},size: {dataSize} and count {count}. Expected >0, {_config.DataSize}, {_config.DataCount}");
                return false;
            }

            message = new TacoMessage(definitions) {IntValues = new int[count]};
            for (int i = 0; i < count; i++)
                message.IntValues[i] = r.ReadInt32();
            char c = default;
            while (r.PeekChar() > -1 && (c = r.ReadChar()) != '|')
                ;
            if (c != '|' || (c = r.ReadChar()) != '\n')
            {
                _logger.LogInformation(
                    $"Error parsing tacomessage. The line end characters were missing nessage values {message.IntValues.Select(v => v.ToString())} with final character {c}");
                return false;
            }

            return true;
        }

        private byte[] buffer = new byte[6000];
        public Task<TacoDefinition[]> ParseDefinitionsAsync(Stream s)
        {
            int bytes = 0;
            int count = 0;
            using (var r = new BinaryReader(new BufferedStream(s), Encoding.ASCII,true))
            {
/*                bool found = false;
                while (!found)
                {
                    for (int i = 0; i < AldlDefinitionMessage.MAGIC_PHRASE_VAL.Length; i++)
                    {
                        if (r.ReadByte() != AldlDefinitionMessage.MAGIC_PHRASE_VAL[i])
                        {
                            found = false;
                            break;
                        }
                        found = true;
                    }
                }*/
                _logger.LogDebug("Reading definition data");
                var ms = r.ReadAsciiString(ref bytes);
                var mse = AldlDefinitionMessage.MAGIC_PHRASE_VAL;
                if(ms != mse)
                    throw new InvalidOperationException($"{bytes},{ms},{mse},{count}");
                var numDef = r.ReadInt32(ref bytes);
                var defSize = r.ReadInt32(ref bytes);
                if(numDef != _config.DataCount)
                    throw new Exception($"Unexpected mismatch between number of definitions and configured {count}, {_config.DataCount}");
                var l = new List<TacoDefinition>();
                for (int i = 0; i < numDef; i++)
                {
                    bytes = 0;
                    var def = new TacoDefinition()
                    {
                        Index = i,
                        Id = r.ReadAsciiString(ref bytes),
                        Unit = r.ReadAsciiString(ref bytes),
                        Description = r.ReadAsciiString(ref bytes),
                    };
                    bytes = 0;
                    // throw away pointers
                    r.ReadPointer(ref bytes);
                    r.ReadPointer(ref bytes);
                    r.ReadPointer(ref bytes);
                    def.log = r.ReadInt32AsBool(ref bytes);
                    def.display = r.ReadInt32AsBool(ref bytes);
                    def.alarm_low_enable= r.ReadInt32AsBool(ref bytes);
                    def.alarm_high_enable= r.ReadInt32AsBool(ref bytes);
                    def.alarm_low.IntVal= r.ReadInt32(ref bytes);
                    def.alarm_high.IntVal= r.ReadInt32(ref bytes);
                    def.type = (AldlDataType) r.ReadInt32(ref bytes);

                    def.min.IntVal = r.ReadInt32(ref bytes);
                    def.max.IntVal = r.ReadInt32(ref bytes);
                    def.adder.IntVal = r.ReadInt32(ref bytes);
                    def.multiplier.IntVal = r.ReadInt32(ref bytes);
                    def.precision = r.ReadByte(ref bytes);
                    def.packet = r.ReadByte(ref bytes);
                    def.offset = r.ReadByte(ref bytes);
                    def.size = r.ReadByte(ref bytes);
                    def.binary = r.ReadByte(ref bytes);
                    def.invert = r.ReadByteAsBool(ref bytes);
                    def.err = r.ReadByte(ref bytes);
                    // padding
                    while (bytes < defSize)
                        r.ReadByte(ref bytes);
                    var p = r.ReadChar(ref bytes);
                    var n = r.ReadChar(ref bytes);
                    if (p != '|' || n != '\n')
                    {
                        var st = $"{p}{n}";
                        while (s.CanRead && (n = r.ReadChar()) != '\n')
                            st += n;
                        throw new InvalidOperationException($"{i},{def.Id}, {st}");
                    }

                    l.Add(def);
                    _logger.LogDebug($"Loaded definition {i} {def.Id}, {def.Description}");
                }

                return Task.FromResult(l.ToArray());
            }
        }

        private void ReadBytes(Stream stream, byte[] buffer, int defSize)
        {
            var read = 0;
            while (read < defSize)
            {
                read += stream.Read(buffer, 0, defSize - read);
            }
        }
    }
}