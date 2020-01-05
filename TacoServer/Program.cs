using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
namespace TacoServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                // `LogEventLevel` requires `using Serilog.Events;`
                .MinimumLevel.Is(LogEventLevel.Verbose)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("log.txt")
                .CreateLogger();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile(
                        "tacogladys.json", optional: false, reloadOnChange: true);
                })
                .ConfigureLogging((context, logging) =>
                    {
//                        logging.AddFile("logs/tacogladys-{Date}.txt");    
                        logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                        logging.AddConsole().AddDebug().AddEventSourceLogger().SetMinimumLevel(LogLevel.Trace);
                    }
                    )
                    .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
