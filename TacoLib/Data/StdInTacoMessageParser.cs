using System;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TacoLib.Interop;

namespace TacoLib.Data
{
    /*
    public class StdInTacoMessageParser : ITacoMessageParser
    {
        private readonly TacoConfiguration _config;

        public StdInTacoMessageParser(TacoConfiguration config)
        {
            _config = config;
        }

        public TacoDefinition[] Defintitions { get; }
        public int BufferSize => _config.MessageSize;
        public Task ParseAldlDefinitions(Stream s) => Task.CompletedTask;

        public TacoMessage ParseMessageInto(TacoMessageBuffer buffer, long formatTime)
        {
            var tokens = Tokenize(buffer);
            return new TacoMessage()
            {
            };

        }

        private static IEnumerable<String> Tokenize(TacoMessageBuffer buffer)
        {
            var msg = Encoding.ASCII.GetString(buffer.Data);
            var text = (ReadOnlySpan<char>) msg;
            var l = new List<string>();
            l.Add(msg);
            while (!text.IsEmpty)
            {
                var ix = text.IndexOf(' ');
                if (ix < 0)
                {
                    l.Add(new string(text));
                    break;

                }

                var token = text.Slice(0, ix);
                if(token.IsEmpty)
                {
                    if (text.Length > 0)
                    {
                        text = text.Slice(1);
                        continue;
                    }

                    break;
                }
                text = text.Slice(ix + 1);
                l.Add(new string(token));
            }
            return l;
        }

        public Task<TacoMessage> FormatAsync(TacoMessageBuffer buff, long formatTime)
        {
            throw new System.NotImplementedException();
        }
    }*/
}