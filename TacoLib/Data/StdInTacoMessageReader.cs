using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TacoLib.Data;
using TacoLib.Interop;

namespace TacoLib
{
    public class StdInTacoMessageReader : RestartableStream
    {
        private Stream _stream;

        public StdInTacoMessageReader(TacoConfiguration config,ITacoMessageParser parser, ILogger<StdInTacoMessageReader> logger) : base(logger)
        {
            _stream = Console.OpenStandardInput();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose();
            _stream.Dispose();
        }

        protected override bool NeedsRestart() => false;

        protected override bool IsDisposing() => false;

        protected override Stream GetInnerStream() => _stream;
        protected override Task RestartStream(bool consecutive, CancellationToken? token = null)
        {
            _stream = Console.OpenStandardInput();
            return Task.CompletedTask;
        }
    }
}