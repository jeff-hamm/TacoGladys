using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using TacoLib.Data;
using TacoLib.Interop;

namespace TacoLib
{
    public class TacoMessageBuffer
    {
        private readonly TacoMessageQueue _queue;

        public TacoMessageBuffer(TacoMessageQueue queue, int size)
        {
            _queue = queue;
            Data = new byte[size];
        }
        protected byte[] Data { get; }
        public int DataLength { get; set; }

        internal Writer GetWriter() => new Writer(this);

        internal struct Writer : IDisposable
        {
            private readonly TacoMessageBuffer _buffer;

            public Writer(TacoMessageBuffer buffer)
            {
                _buffer = buffer;
                _complete = false;
            }

            public byte[] Data => _buffer.Data;

            private bool _complete;
            public async Task CompleteAsync(int bytesWritten)
            {
                _buffer.DataLength = bytesWritten;
                await _buffer._queue.AddBuffer(_buffer).ConfigureAwait(false);
                _complete = true;
            }
            public void Dispose()
            {
                if(_complete)
                    _buffer._queue.FreeMessageBuffer(_buffer);
            }
        }

        public bool TryParseMessage(ITacoMessageParser parser, TacoDefinitions definitions, out TacoMessage message) =>
            parser.TryParseMessage(Data, DataLength,definitions, out message);

    }
}