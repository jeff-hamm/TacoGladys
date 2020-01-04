using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TacoLib.Data;

namespace TacoLib.Interop
{
    public class FileTacoMessageReader : RestartableStream 
    {
        private readonly TacoConfiguration _config;
        public FileStream FileStream { get; private set; }

        public FileTacoMessageReader(TacoConfiguration config, ILogger<FileTacoMessageReader> logger) : base(logger)
        {
            _config = config;
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose();
            FileStream.Dispose();
        }

        protected override bool NeedsRestart() =>
            !FileStream.CanRead;

        protected override bool IsDisposing() =>
            !FileStream.CanRead;

        protected override Stream GetInnerStream() => FileStream;

        protected override Task RestartStream(bool consecutive, CancellationToken? token = null)
        {
            FileStream = System.IO.File.OpenRead(_config.TacoStreamFileName);
            return Task.CompletedTask;
        }
    }
}