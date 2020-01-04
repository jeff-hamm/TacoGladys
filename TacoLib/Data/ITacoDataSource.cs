using System;
using System.Threading;
using System.Threading.Tasks;
using TacoLib.Interop;

namespace TacoLib.Data
{
    public interface ITacoDataSource
    {
        Task<TacoDefinition[]> GetDefinitionsAsync(CancellationToken token = default);
        Task<TacoMessage> ReadMessageAsync(CancellationToken token = default);
    }
}