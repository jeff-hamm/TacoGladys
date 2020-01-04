using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using TacoLib.Data;
using TacoLib.Extensions;
using TacoLib.Interop;

namespace TacoLib
{
    public class TacoMessageReaderRunner : IDisposable
    {
        public TacoMessageReader MessageReader { get; }
        private readonly TacoConfiguration _config;
        private readonly TacoMessageQueue _messageQueue;
        private readonly TacoDefinitions _definitions;
        private readonly ITacoMessageParser _parser;
        private readonly ILogger<TacoMessageReaderRunner> _logger;
        public DateTime RunStarted { get; private set; }
        public bool IsRunning => _readTask?.IsCompleted == false;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationTokenSource _disposeSource = new CancellationTokenSource();
        private readonly CancellationToken _disposeToken;


        private Task _readTask;

        public TacoMessageReaderRunner(TacoConfiguration config, 
            TacoMessageReader messageReader, 
            TacoMessageQueue messageQueue,
            TacoDefinitions definitions,
            ITacoMessageParser parser,
            ILogger<TacoMessageReaderRunner> logger)
        {
            MessageReader = messageReader;
            _config = config;
            _messageQueue = messageQueue;
            _definitions = definitions;
            _parser = parser;
            _logger = logger;
            _disposeToken = _disposeSource.Token;

        }

        
        public async Task StartReading()
        {
            if(!IsRunning)
                await Run(_disposeToken);
        }

        public async Task<TacoMessage> GetMessageAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            _disposeToken.ThrowIfCancellationRequested();
            using var ls = CancellationTokenSource.CreateLinkedTokenSource(token,_disposeToken);
            token = ls.Token;
            while (true)
            {
                token.ThrowIfCancellationRequested();
                try
                {
                    if (!IsRunning)
                        await StartReading();
                    TacoMessageBuffer msgBuffer = null;
                    try
                    {
                        msgBuffer = await _messageQueue.TakeUsedMessage(token);
                        if (msgBuffer.TryParseMessage(_parser, _definitions, out var message))
                            return message;
                        _logger.LogWarning($"Ignoring bad message, continuing");
                    }
                    finally
                    {
                        if(msgBuffer != null)
                            _messageQueue.FreeMessageBuffer(msgBuffer);
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"There was an error parsing the next message. {ex} ");
                }
            }
        }


        private void StartRunInternal(CancellationToken token)
        {
            if (_readTask != null) throw new Exception($"Unexpected error: message reader task already exists");
            _logger.LogTrace("Running Stream task");
            _readTask = Task.Run(async () =>
            {
                if(!token.IsCancellationRequested)
                    await RunInternalAsync(token);
            }, token).ContinueWith((t) => ReadTaskComplete(t,token));
        }

        private async Task ReadTaskComplete(Task t, CancellationToken originalToken)
        {
            if (originalToken.IsCancellationRequested) return;
            if (t.IsCanceled) return;
            else if (t.IsFaulted)
            {
                var restartDelay = _config.MessageReaderFailedRetryDelayS;
                _logger.LogError(
                    $"Message Reader task faulted: {t.Exception?.ToString()}. Delaying {restartDelay} seconds and trying again");
                _readTask?.Dispose();
                _readTask = null;
                await Task.Delay(restartDelay);
                StartRunInternal(originalToken);
            }
            else
                throw new Exception(
                    $"Reader task completed gracefully, this should not happen. It should run forever, be cancelled or fault");
        }


        public Stopwatch ReadTimer { get; } = new Stopwatch();
        public long LastRead { get; private set; }
        public long LastFailure { get; private set; }
        public int FailedReads { get; private set; }
        private async Task RunInternalAsync(CancellationToken token)
        {
            FailedReads = 0;
            ReadTimer.Restart();
            try
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    try
                    {
                        if (MessageReader.IsDisposed)
                            throw new Exception(
                                $"Message reader was disposed while being read. This should not happen. Do not dispose the message reader until it is not being read.");
                        await foreach (var messageData in MessageReader.EnumerateStreamMessagesAsync(token)
                            .ConfigureAwait(false))
                        {
                            _logger.LogTrace($"Adding new message");
                            LastRead = ReadTimer.ElapsedTicks;
                            token.ThrowIfCancellationRequested();
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogInformation($"Message stream was cancelled");
                        throw;
                    }
                    catch (StreamRestartingException)
                    {
                        _logger.LogInformation($"Message  stream is restarting");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Got an error while reading stream {ex}. Failed Reads: {FailedReads} Restarting");
                        FailedReads++;
                        LastFailure = ReadTimer.ElapsedTicks;
                    }
                }
            }
            finally
            {
                ReadTimer.Stop();
            }
        }




        private AsyncLock _runLock = new AsyncLock();
        public async Task Run(CancellationToken token)
        {
            if (IsRunning) return;
            using (await _runLock.LockAsync(token))
            {

            }
            {
                if (IsRunning) return;
                RunStarted = DateTime.Now;
                if(_cancellationTokenSource != null)
                    throw new Exception($"Unexpected error, internal token cancellation source not null");
                _cancellationTokenSource = token.CreateChildCancellationSource();
                token.Register(Cancel);
                StartRunInternal(_cancellationTokenSource.Token);
            }
        }
        public void Cancel()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = null;
            if (!IsRunning) return;
            using (_runLock.Lock())
            {
                if (!IsRunning) return;
            }
        }

        private bool IsDisposed { get; set; }
        public void Dispose()
        {
            IsDisposed = true;
            Cancel();
            _disposeSource.Cancel();
            _disposeSource.Dispose();
        }
    }
}