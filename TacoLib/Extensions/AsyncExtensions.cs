using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TacoLib.Extensions
{
    public static class CancellationTokenExtensions
    {
        public static CancellationTokenSource CreateChildCancellationSource(this CancellationToken @this)
        {
            var source = new CancellationTokenSource();
            @this.Register(source.Cancel);
            return source;
        }
        public static CancellationTokenSource CreateLinkedCancellationSource(this CancellationToken @this, params CancellationToken[] tokens) =>
            CancellationTokenSource.CreateLinkedTokenSource(tokens);

        public static CancellationToken CreateLinkedToken(this CancellationToken @this,
            params CancellationToken[] tokens)
        {
            var s = CancellationTokenSource.CreateLinkedTokenSource(tokens);
            var t = s.Token;
            t.Register(s.Cancel);
            return t;
        }

    }
}
