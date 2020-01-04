using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using TacoLib.Data;
using TacoLib.Interface;
using TacoLib.Tests.data;

namespace TacoServer.Pages
{
    public class TacoGladysModel : PageModel
    {
        private readonly ILogger<TacoGladysModel> _logger;
        private readonly ITacoDataSource _dataSource;

        public TacoGauges Gauges { get; private set; }
        public TacoGaugeLayout[] Layouts { get; } = new[]
        {
            new TacoGaugeLayout(TacoValueId.RPM),
            new TacoGaugeLayout(TacoValueId.SPEED),
            new TacoGaugeLayout(TacoValueId.COOLTMP),
            new TacoGaugeLayout(TacoValueId.LBPW)
            {
                Min = 0.0,
                Max = 7.0
            },
            new TacoGaugeLayout(TacoValueId.RBPW)
            {
                Min = 0.0,
                Max = 7.0
            },
            new TacoGaugeLayout(TacoValueId.BLM, GaugeType.Text),
            new TacoGaugeLayout(TacoValueId.LBLM, GaugeType.Text),
            new TacoGaugeLayout(TacoValueId.RBLM, GaugeType.Text),
            new TacoGaugeLayout(TacoValueId.LINT, GaugeType.Text),
            new TacoGaugeLayout(TacoValueId.RINT, GaugeType.Text),

        };

        public TacoGladysModel(ILogger<TacoGladysModel> logger, ITacoDataSource dataSource)
        {
            _logger = logger;
            _dataSource = dataSource;
        }

        public async Task OnGet()
        {
            Gauges = new TacoGauges(Layouts, await _dataSource.GetDefinitionsAsync());

        }
    }
}
