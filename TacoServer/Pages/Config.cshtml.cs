using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
    public class ConfigGladysModel : PageModel
    {
        private readonly ILogger<ConfigGladysModel> _logger;
        private readonly TacoConfigurationService _configService;

        public TacoConfiguration Config => _configService.Config;

        public ConfigGladysModel(ILogger<ConfigGladysModel> logger, TacoConfigurationService configService)
        {
            _logger = logger;
            _configService = configService;
        }

        [BindProperty]
        public string ConfigJson { get; set; }

        public void OnGet()
        {
            ConfigJson = JsonSerializer.Serialize(Config, new JsonSerializerOptions()
            {
                WriteIndented = true
            });
        }

        public async Task OnPost(string configJson)
        {
            _configService.WriteConfig(JsonSerializer.Deserialize<TacoConfiguration>(configJson));
        }
    }
}
