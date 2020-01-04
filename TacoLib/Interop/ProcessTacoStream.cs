using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using Nito.AsyncEx.Synchronous;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TacoLib.Interop
{
    public class ProcessTacoStream : RestartableStream
    {
        private readonly TacoConfiguration _config;
        private readonly TacoStreamType _streamType;
        private readonly ILogger<ProcessTacoStream> _logger;
        private readonly CancellationTokenSource _disposeSource = new CancellationTokenSource();
        private readonly CancellationToken _disposeToken;
        public Process Process { get; private set; }

        public ProcessTacoStream(TacoConfiguration config, TacoStreamType streamType, ILogger<ProcessTacoStream> logger) : base(logger)
        {
            _config = config;
            _streamType = streamType;
            _logger = logger;
            _disposeToken = _disposeSource.Token;
        }

        private AsyncLock _openLock = new AsyncLock();
        public override async Task Open(CancellationToken token) {
            if (token.IsCancellationRequested || _disposeToken.IsCancellationRequested) return;
            if (Process == null || !IsOpen)
                using(await _openLock.LockAsync(token))
                    BeginProcess(GetOpenToken(token));
            else if (Process.HasExited)
            {
                _logger.LogDebug($"Waiting for proecess to start {Process.Id}");
                await Task.Delay(50, token).ConfigureAwait(false);
                if (Process.HasExited)
                    using(await _openLock.LockAsync(token))
                        BeginProcess(GetOpenToken(token));
            }

        }

        private CancellationToken? _processToken;
        private CancellationToken GetOpenToken(CancellationToken openToken)
        {
            if (openToken.IsCancellationRequested) return openToken;
            if (_disposeToken.IsCancellationRequested) return _disposeToken;
            if (_processToken.HasValue) return _processToken.Value;
            if (openToken != default && openToken != CancellationToken.None)
                return (_processToken = openToken).Value;
            return _disposeToken;
        }


        private readonly AsyncLock _beginLock = new AsyncLock();
        protected Process BeginProcess() => BeginProcess(_processToken ?? default);
        protected Process BeginProcess(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            _disposeToken.ThrowIfCancellationRequested();
            ClearIfExited();
            if (Process?.HasExited == false) 
                return Process;
            lock (_processLock)
            {
                if (Process?.HasExited == false) 
                    return Process;
                _innerStream?.Dispose();
                _innerStream = null;
                Process = new Process()
                {
                    StartInfo = new ProcessStartInfo(_config.AldlIoPath)
                    {
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        Arguments = "-" + _streamType.ToString().ToLower()[0],
                        RedirectStandardError = true,
                        UseShellExecute = false,
                    },
                    EnableRaisingEvents = true
                };
                Process.Exited += ProcessOnExited;
                Process.ErrorDataReceived += ProcessOnErrorDataReceived;
                Process.Disposed += Process_Disposed;
                _logger.LogDebug($"Starting {_streamType}");
                var started = Process.Start();
                _logger.LogDebug($"Process {Process?.Id} {_streamType} has been started");
                if (!IsOpen)
                {
                    //you may allow for the process to be re-used (started = false) 
                    //but I'm not sure about the guarantees of the Exited event in such a case
                    throw new InvalidOperationException("Could not start process: " + Process);
                }

                return Process;
            }
        }

        public override bool IsOpen => !_disposing && Process?.HasExited == false;

        private void ClearIfExited()
        {
            if (Process != null && Process.HasExited)
            {
                lock(_processLock)
                {
                    if (Process != null && Process.HasExited)
                    {
                        _logger.LogDebug($"Process is being cleared");
                        Process.Exited -= ProcessOnExited;
                        Process.ErrorDataReceived -= ProcessOnErrorDataReceived;
                        Process.Disposed -= Process_Disposed;
                        Process?.Dispose();
                        _innerStream?.Dispose();
                        _innerStream = null;
                        Process = null;
                        _processToken = null;
                    }
                }
            }
        }

        private void Process_Disposed(object sender, EventArgs e)
        {
            if (IsOpen) throw new Exception($"TacoStream Process was disposed before TacoStream");
            _logger.LogDebug($"Process {Process?.Id} {_streamType} has been disposed");
        }

        private void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            _logger.LogError($"StdError from TacoStream {_streamType}: {e.Data}");
        }

        private void ProcessOnExited(object sender, EventArgs e)
        {
            if (!_disposing && !_disposeSource.IsCancellationRequested)
            {
                _logger.LogError(
                    $"The TacoStream {_streamType} process exited. Attempting to restart it.");
                TriggerRestart();
            }
            else
                _logger.LogDebug($"Process {Process?.Id} {_streamType} is has closed");
        }

        public int ConsecutiveRestartErrors { get; private set; }

        protected override bool NeedsRestart() => Process?.StandardOutput?.BaseStream == null || Process == null ||
                                                Process.HasExited == true;
        protected override bool IsDisposing() => _disposing || _disposeToken.IsCancellationRequested ||
                                               (_processToken?.IsCancellationRequested ?? false);

        private BufferedStream _innerStream;
        private object _processLock = new object();
        protected override Stream GetInnerStream()
        {
            if (_innerStream != null) return _innerStream;
            lock (_processLock)
            {
                if (_innerStream != null) return _innerStream;
                _innerStream = new BufferedStream(Process?.StandardOutput?.BaseStream);
            }

            return _innerStream;
        } 

        protected override async Task RestartStream(bool consecutive, CancellationToken? token=null)
        {
            if(consecutive)
                await Task.Delay(_config.TacoStreamProcessRestartS);

                // the existing token should still be registered
            BeginProcess(token ?? _processToken ?? default);
        }

        private bool _disposing;
        protected override void Dispose(bool disposing)
        {
            _disposing = true;
            Process?.StandardInput.BaseStream.WriteByte((byte)' ');
            _disposeSource.Cancel();
            base.Dispose(disposing);
            if (Process != null)
            {
                if(IsOpen)
                    _logger.LogDebug($"Process {Process?.Id} {_streamType} is being closed");
                _innerStream?.Dispose();
                Process.Disposed -= Process_Disposed;
                Process.Dispose();
            }
        }
    }
}
