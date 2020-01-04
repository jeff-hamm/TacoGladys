using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TacoLib.Interop
{
    public enum TacoStreamType
    {
        Message,
        Definition
    }

    public static class TacoStreamExtensions
    {
        public static Task<RestartableStream> MessagesAsync(this ITacoStreamFactory @this, CancellationToken token) =>
            @this.GetOrOpenAsync(TacoStreamType.Message,token);
        public static  async Task<RestartableStream> DefinitionsAsync(this ITacoStreamFactory @this, CancellationToken token) =>
            await @this.GetOrOpenAsync(TacoStreamType.Definition,token).ConfigureAwait(false);
    }
    public interface ITacoStreamFactory : IDisposable
    {
        Task<RestartableStream> GetOrOpenAsync(TacoStreamType streamType, CancellationToken openToken);
    }

    public class ProcessTacoStreamFactory : TacoStreamFactory
    {
        private readonly TacoConfiguration _config;
        private readonly ILogger<ProcessTacoStream> _logger;

        public ProcessTacoStreamFactory(TacoConfiguration config, ILogger<ProcessTacoStream> logger) : base(config, logger)
        {
            _config = config;
            _logger = logger;
        }

        protected override Task<RestartableStream> CreateStream(TacoStreamType t) =>
                Task.FromResult((RestartableStream) new ProcessTacoStream(_config, t, _logger));
    }

    public abstract class TacoStreamFactory : ITacoStreamFactory
    {
        private readonly TacoConfiguration _config;
        private readonly ILogger<ProcessTacoStream> _logger;

        public TacoStreamFactory(TacoConfiguration config, ILogger<ProcessTacoStream> logger)
        {
            _config = config;
            _logger = logger;
        }

        private readonly ConcurrentDictionary<TacoStreamType, Task<RestartableStream>> _streams =
            new ConcurrentDictionary<TacoStreamType, Task<RestartableStream>>();

        public async Task<RestartableStream> GetOrOpenAsync(TacoStreamType streamType, CancellationToken token = default)
        {
            if (_disposing) throw new Exception($"Cannot open a tacostream will disposing");
            var s = await _streams.GetOrAdd(streamType, async (t) => await CreateStream(t));
            if (!s.IsOpen)
                await s.Open(token);
            return s;
        }

        protected abstract Task<RestartableStream> CreateStream(TacoStreamType t);

        private bool _disposing;
        public void Dispose()
        {
            _disposing = true;
            foreach (var s in _streams.Values)
                s.Dispose();
        }

    }
}