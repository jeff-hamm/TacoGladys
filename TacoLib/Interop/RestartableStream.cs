using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TacoLib.Interop
{
    public abstract class RestartableStream : Stream
    {
        private readonly ILogger _logger;

        protected RestartableStream(ILogger logger)
        {
            _logger = logger;
            _disposeToken = _disposeTokenSource.Token;
        }

        protected Stream InnerStream => GetStream();
        protected abstract bool NeedsRestart();
        protected abstract bool IsDisposing();
        public virtual bool IsOpen => !IsDisposing();
        protected abstract Stream GetInnerStream();
        protected abstract Task RestartStream(bool consecutive,CancellationToken? token=null);

        public async Task WaitForRestart(CancellationToken token)
        {
            while (!IsDisposing() && (IsRestarting || NeedsRestart()) && !token.IsCancellationRequested && !_disposeToken.IsCancellationRequested)
                await Task.Delay(50, token);
        }

        public virtual Task Open(CancellationToken token) =>
            RestartStream(false, token);
        public bool IsRestarting { get; private set; }
        public int RestartErrors { get; private set; }
        public Exception LastRestartException { get; private set; }
        private async Task EnsureRestartAsync(bool consecutive)
        {
            try
            {
                if (!consecutive)
                    RestartErrors = 0;
                else
                    RestartErrors++;
                IsRestarting = true;
                await RestartStream(consecutive);
                RestartErrors = 0;
            }
            catch (OperationCanceledException)
            {
                throw;

            }
            catch (Exception ex)
            {
                LastRestartException = ex;
                _logger.LogError($"Error restarting stream {ex}");
                _disposeToken.ThrowIfCancellationRequested();
                await EnsureRestartAsync(true);
            }
            finally
            {

                IsRestarting = false;
            }

        }

        private readonly CancellationTokenSource _disposeTokenSource = new CancellationTokenSource();
        private readonly CancellationToken _disposeToken;
        private Task _restartTask;
        protected virtual Stream GetStream()
        {
            if (IsDisposing()) throw new OperationCanceledException($"Stream is shutting down.");
            if (NeedsRestart())
            {
                TriggerRestart();
                throw new StreamRestartingException();
            }
            return GetInnerStream();
        }

        protected void TriggerRestart()
        {
            if (IsRestarting) return;
            if (_restartTask != null)
            {
                if (!_restartTask.IsCompleted) return;
                if (_restartTask.IsFaulted)
                    _logger.LogError($"Error restarting stream: {_restartTask?.Exception}");
                _restartTask = null;
            }

            _restartTask = Task.Run(async () => await EnsureRestartAsync(false), _disposeToken);
        }


        public override void Flush()
        {
            InnerStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return InnerStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return InnerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            InnerStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            InnerStream.Write(buffer, offset, count);
        }

        public override bool CanRead => InnerStream.CanRead;

        public override bool CanSeek => InnerStream.CanSeek;

        public override bool CanWrite => InnerStream.CanWrite;

        public override long Length => InnerStream.Length;

        public override long Position
        {
            get => InnerStream.Position;
            set => InnerStream.Position = value;
        }
    }

    public class StreamRestartingException : Exception
    {
    }
}