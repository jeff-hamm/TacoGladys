using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using TacoLib;
using TacoLib.Data;
using TacoLib.Interop;

namespace TacoServer.Hubs
{
    public class TacoGladysHub : Hub
    {
        private readonly ITacoDataSource _ds;
        private readonly ILogger<TacoGladysHub> _logger;

        public TacoGladysHub(ITacoDataSource ds, ILogger<TacoGladysHub> logger)
        {
            _ds = ds;
            _logger = logger;
        }

        public async Task SetGaugeIds(IEnumerable<string> gaugeIds)
        {
            Context.Items["gaugeIds"] = gaugeIds.ToArray();
            Context.Items["definitions"] = await GetDefinitions(gaugeIds).ConfigureAwait(false);
        }

        private async Task<TacoDefinition[]> GetDefinitions(IEnumerable<string> gaugeIds, CancellationToken token=default)
        {
            if (Context.Items["definitions"] is TacoDefinition[] defs) return defs;
            gaugeIds ??= Context.Items["gaugeIds"] as string[];
            defs = await _ds.GetDefinitionsAsync(token).ConfigureAwait(false);
            if (gaugeIds?.Any() == true)
                defs = defs.Where(d => gaugeIds.Contains(d.Id))?.ToArray();
            Context.Items["definitions"] = defs;
            return defs;

        }

        public async IAsyncEnumerable<TacoMessage> Taco([EnumeratorCancellation]CancellationToken token)
        {
            while (!token.IsCancellationRequested)
                yield return await _ds.ReadMessageAsync(token);
        }
        public async IAsyncEnumerable<IDictionary<string,object>> TacoGauges(
            string[] gaugeIds,
            [EnumeratorCancellation]CancellationToken token)
        {
            var defs = await GetDefinitions(gaugeIds,token);
            await foreach (var message in Taco(token))
            {
                if (message == null) continue;
                yield return defs.ToDictionary(d => d.Id, d => message[d]);
            }
        }


    }
}
