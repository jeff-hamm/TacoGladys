using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TacoLib.Interop;

namespace TacoLib.Data
{
    public interface ITacoMessageParser
    {
        Task<TacoDefinition[]> ParseDefinitionsAsync(Stream s);
        bool TryParseMessage(byte[] buffer, int length, TacoDefinitions definitions, out TacoMessage message);
    }
}