using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using TacoLib;
using TacoLib.Data;
using TacoLib.Interface;
using TacoLib.Tests.data;

namespace TacoServer.Pages
{
    public class TacoGladysModel : PageModel
    {
        private readonly ILogger<TacoGladysModel> _logger;
        private readonly ITacoDataSource _dataSource;
        public TacoGauges Gauges { get; }

        public TacoGladysModel(ILogger<TacoGladysModel> logger, ITacoDataSource dataSource, TacoGauges gauges)
        {
            _logger = logger;
            _dataSource = dataSource;
            Gauges = gauges;
        }

        public async Task OnGet()
        {
            await Gauges.EnsureConfiguredAsync();
        }
    }
}
