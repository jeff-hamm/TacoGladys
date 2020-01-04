using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using TacoLib.Data;
using TacoLib.Extensions;
using TacoLib.Interop;

namespace TacoLib
{
    public class TacoMessageQueue
    {
        private readonly TacoConfiguration _config;
        private readonly TacoDefinitions _definitions;
        private readonly ILogger<TacoMessageQueue> _logger;


        public TacoMessageQueue(TacoConfiguration config, 
            TacoDefinitions definitions, 
            ILogger<TacoMessageQueue> logger)
        {
            _config = config;
            _definitions = definitions;
            _logger = logger;
        }

        private bool _initialized = false;
        protected async Task InitBuffers(CancellationToken token)
        {
            if (_initialized) return;
            var defs = await _definitions.GetDefinitionsAsync(token);
            for (int i = 0; i < _config.MaxBufferCount; i++)
                _freeBuffers.Push(new TacoMessageBuffer(this,_config.DataBufferSize));
            _initialized = true;
        }

        public async Task<TacoMessageBuffer> TakeUsedMessage(CancellationToken token) =>
            await _usedBuffers.TakeAsync(token);


        private readonly AsyncCollection<TacoMessageBuffer> _usedBuffers = new AsyncCollection<TacoMessageBuffer>();
        private readonly ConcurrentStack<TacoMessageBuffer> _freeBuffers = new ConcurrentStack<TacoMessageBuffer>();
        internal void FreeMessageBuffer(TacoMessageBuffer buffer)
        {
            _freeBuffers.Push(buffer);
        }

        internal async Task<TacoMessageBuffer> GetFreeBufferAsync(CancellationToken token)
        {
            if (!_initialized)
                await InitBuffers(token);
            if (_freeBuffers.TryPop(out var data)) return data;
            return (await _usedBuffers.TakeAsync(token).ConfigureAwait(false));
        }

        public async Task AddBuffer(TacoMessageBuffer buffer)
        {
            await _usedBuffers.AddAsync(buffer);
        }
    }

    public class TacoMessageReader : IDisposable
    {
        private readonly ITacoStreamFactory _factory;
        private readonly TacoMessageQueue _messageQueue;
        private readonly ILogger _logger;
        internal ITacoMessageParser Parser { get; }

        protected TacoConfiguration Config { get; }

        public TacoMessageReader(TacoConfiguration config,ITacoStreamFactory _factory,  
            ITacoMessageParser parser, TacoMessageQueue messageQueue, ILogger<TacoMessageReader> logger)
        {
            this._factory = _factory;
            _messageQueue = messageQueue;
            _logger = logger;
            Parser = parser;
            Config = config;

            _disposeToken = _disposeCancellationSource.Token;
        }



        private readonly CancellationTokenSource _disposeCancellationSource = new CancellationTokenSource();
        private readonly CancellationToken _disposeToken;
        public bool IsIdle { get; private set; }
        public int FailedReads { get; private set; }


        public async IAsyncEnumerable<TacoMessageBuffer> EnumerateStreamMessagesAsync([EnumeratorCancellation] CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            _disposeToken.ThrowIfCancellationRequested();
            _logger.LogTrace($"Starting message stream");
            using (var tokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(_disposeToken, token))
            {
                try
                {
                    token = tokenSource.Token;
                    var stream = await _factory.MessagesAsync(token);
                    if (!stream.CanRead) throw new InvalidOperationException();
                    while (!token.IsCancellationRequested)
                        yield return await ReadMessageDataFromInternalStream(stream, token);
                }
                finally
                {
                    _logger.LogError("Exiting message stream.");
                    IsIdle = false;
                }
            }
        }


        private async Task<TacoMessageBuffer> ReadMessageDataFromInternalStream(Stream stream,CancellationToken token)
        {
            FailedReads = 0;
            token.ThrowIfCancellationRequested();
            var messageData = await _messageQueue.GetFreeBufferAsync(token).ConfigureAwait(false);
            using var writer = messageData.GetWriter();
            int length = 0;
            while (!ReadInto(stream, writer.Data, out length))
            {
                FailedReads++;
                IsIdle = true;
                await IdleAsync(token).ConfigureAwait(false);
            }
            await writer.CompleteAsync(length).ConfigureAwait(false);
            return messageData;
        }


        private async Task IdleAsync(CancellationToken token)
        {
            _logger.LogDebug("Idling");
            if (FailedReads > Config.MaxIdleMs)
                FailedReads = Config.MaxIdleMs;
            await Task.Delay(FailedReads, token).ConfigureAwait(false);
        }




        public bool ReadInto(Stream stream, byte[] messageBuffer, out int length)
        {
            length = 0;
            while (length < messageBuffer.Length)
                length += stream.Read(messageBuffer, 0, messageBuffer.Length - length);

            if (messageBuffer[^2] != '|' || messageBuffer[^1] != '\n')
            {
                while (true)
                {
                    while (stream.ReadByte() != (int) '|')
                        ;
                    if (stream.ReadByte() == (int) '\n')
                        break;
                }

                return false;
            }

            return true;
        }

        internal bool IsDisposed { get; private set; }
        public virtual void Dispose()
        {
            _disposeCancellationSource.Cancel();
            IsDisposed = true;
        }


    }
}