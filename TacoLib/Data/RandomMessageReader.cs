using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TacoLib.Data;
using TacoLib.Interop;

namespace TacoLib
{
    public class RandomMessageReader : RestartableStream
    {
        private RandomStream _stream;

        private class RandomStream : MemoryStream
        {
            private Random r = new Random();
            public override int Read(byte[] buffer, int offset, int count)
            {
                count = r.Next(0, count);
                for (int i = 0; i < count; i++)
                    buffer[i+offset] = (byte)ReadByte();
                return count;
            }

            public override int Read(Span<byte> destination)
            {
                var count = r.Next(0, destination.Length);
                for (int i = 0; i < count; i++)
                    destination[i] = (byte)ReadByte();
                return count;
            }

            public override int ReadByte() =>
                (byte) r.Next((int) '0', (int) 'Z');
        }
        public RandomMessageReader(ILogger<RandomMessageReader> logger) : base(logger)
        {
            _stream = new RandomStream();

        }

        protected override bool NeedsRestart() => false;
        protected override bool IsDisposing() => false;
        protected override Stream GetInnerStream() => _stream;
        protected override Task RestartStream(bool consecutive, CancellationToken? token = null)
        => Task.CompletedTask;
    }
}