using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TacoLib.Data;

namespace TacoLib
{/*
    public class TacoSession : IDisposable
    {
        public Guid SessionId { get; } = Guid.NewGuid();
        private readonly TacoMessageReader _dataSource;
        private readonly CancellationTokenSource _disposeTokenSource = new CancellationTokenSource();
        private readonly CancellationToken _disposeToken;
        public TacoSession(TacoMessageReader dataSource, CancellationToken containerToken)
        {
            if (containerToken != default && containerToken != CancellationToken.None)
            {
                var linked = CancellationTokenSource
                    .CreateLinkedTokenSource(_disposeTokenSource.Token, containerToken);
                _disposeToken = linked.Token;
                _disposeToken.Register(() => linked.Dispose());
            }
            else
                _disposeToken = _disposeTokenSource.Token;
            _dataSource = dataSource;
        }

        public TResult LinkToken<TResult>(CancellationToken token,Func<CancellationToken, TResult> f)
        {
            if (token.IsCancellationRequested) return f(token);
            if (_disposeToken.IsCancellationRequested) return f(_disposeToken);
            if (token == CancellationToken.None || token == default)
                return f(_disposeToken);
            using (var linked =
                CancellationTokenSource.CreateLinkedTokenSource(_disposeTokenSource.Token, token))
                return f(linked.Token);
        }

        public async Task<TacoMessage> ReadNextAsync(CancellationToken token)
        {
            var message = await _dataSource.ReadMessageAsync(token);
            while(!hasNext || _dataSource.Current == null)
            {
                hasNext = await _dataSource.MoveNextAsync().ConfigureAwait(false);
            }
            CurrentMessage = _dataSource.Current;
            Count++;
            if (FirstMessage == null)
                FirstMessage = DateTime.Now;
            return CurrentMessage;
        }

        public DateTime? FirstMessage { get; private set; }
        public TacoMessage CurrentMessage { get; protected set; }
        public string Name { get; set; } = "TacoSession";
        public TacoSession ParentSession { get; set; }
        public ICollection<TacoSession> ChildSessions { get;  } = new List<TacoSession>();
        public long Count { get; set; }
        public DateTime? End { get; private set; }
        public bool IsOpen { get; private set; } = true;

        public void Dispose()
        {
            End = DateTime.Now;
            IsOpen = false;
            if(ChildSessions != null)
                foreach(var session in ChildSessions)
                    session?.Dispose();
        }
    }*/
}