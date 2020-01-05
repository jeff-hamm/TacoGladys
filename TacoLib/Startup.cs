using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TacoLib.Data;
using TacoLib.Interop;
using TacoLib.Tests.data;

[assembly: InternalsVisibleTo("TestLib.Tests")]
namespace TacoLib
{
    public class TacoConfigurationOptions
    {
        public TacoConfiguration Config { get; set; }
        public IConfiguration Section { get; set; }
        public string ConfigFileName { get; set; } = "appsettings.json";
    }
    public static class TacoLibStartup
    {

        public static TacoConfigurationOptions AddConfigurationFile(this TacoConfigurationOptions @this, string fileName)
        {
            @this.ConfigFileName = fileName;
            return @this;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public static void AddTacoServices(this IServiceCollection services, Action<TacoConfigurationOptions> optionsConfig=null)
        {
            var options = new TacoConfigurationOptions()
            {
                Config = new TacoConfiguration()
            };
            optionsConfig?.Invoke(options);
            services.AddSingleton(c => new TacoConfigurationService(options));
            services.AddTransient(c => c.GetRequiredService<TacoConfigurationService>().Config);
            services.AddSingleton<TacoGauges>();
            services.AddTransient<AldlStreamDataSource>();
            services.AddSingleton<ITacoDataSource>(c => c.GetService<AldlStreamDataSource>());
            services.AddTransient<ProcessTacoStreamFactory>();
            services.AddTransient<AldlIoMessageParser>();
            services.AddSingleton<ITacoMessageParser>(c => c.GetService<AldlIoMessageParser>());
            services.AddSingleton<TacoDefinitions>();
            services.AddSingleton<TacoMessageReader>();
            services.AddSingleton<TacoMessageReaderRunner>();
            services.AddSingleton<TacoMessageQueue>();
            services.AddSingleton<ITacoStreamFactory>(c => c.GetService<ProcessTacoStreamFactory>());
        }



    }
}
