using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using Nito.AsyncEx.Synchronous;
using TacoLib.Data;
using TacoLib.Interop;

namespace TacoLib
{
    public class TacoDefinitions : ICollection, IReadOnlyCollection<TacoDefinition>
    {
        private readonly ITacoStreamFactory _factory;
        private readonly ITacoMessageParser _parser;
        private readonly ILogger<TacoDefinitions> _logger;

        public TacoDefinitions(TacoConfiguration config, ITacoStreamFactory factory,
            ITacoMessageParser parser, ILogger<TacoDefinitions> logger)
        {
            this._factory = factory;
            _parser = parser;
            _logger = logger;
        }

        internal TacoDefinition[] Definitions => _definitions ?? throw new Exception($"No definitions loaded. Did you call LoadDefinitions?");

        public TacoDefinition[] GetDefinitionsSync()
        {
            if (Loaded) return _definitions;
            return Task.Run(GetDefinitionsSync)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        public async Task LoadDefinitionsAsync(CancellationToken token = default) =>
            _definitions = await GetDefinitionsAsync(token).ConfigureAwait(false);

        private AsyncLock _defLock = new AsyncLock();
        public async Task<TacoDefinition[]> GetDefinitionsAsync(CancellationToken token = default)
        {
            if (Loaded) return _definitions;
            using(await _defLock.LockAsync(token))
                if (Loaded) return _definitions;
            {
                token.ThrowIfCancellationRequested();
                _logger.LogTrace($"Loading aldl definitions");
                return await GetDefinitionsInternal(token);
            }

        }

        private async Task<TacoDefinition[]> GetDefinitionsInternal(CancellationToken token)
        {
            using (var stream = await _factory.DefinitionsAsync(token).ConfigureAwait(false))
            {
                try
                {
                    _definitions = await _parser.ParseDefinitionsAsync(stream);
                    return _definitions;
                }
                catch (StreamRestartingException)
                {
                    _logger.LogInformation($"Stream restarted while reading definitons");
                    await stream.WaitForRestart(token).ConfigureAwait(false);
                    return await GetDefinitionsInternal(token).ConfigureAwait(false);
                }
            }
        }

        public bool Loaded => _definitions != null;
        private TacoDefinition[] _definitions;

        public IEnumerator<TacoDefinition> GetEnumerator() =>
            Definitions.AsEnumerable().GetEnumerator();


        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        void ICollection.CopyTo(Array array, int index)
        {
            Definitions.CopyTo(array, index);
        }

        public int Count => Definitions.Length;
        bool ICollection.IsSynchronized => Definitions.IsSynchronized;

        object ICollection.SyncRoot => Definitions.SyncRoot;

        public TacoDefinition this[int index] => Definitions[index];

    }
}