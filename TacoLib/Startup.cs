using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using TacoLib.Data;
using TacoLib.Interop;

[assembly: InternalsVisibleTo("TestLib.Tests")]
namespace TacoLib
{
    public static class TacoLibStartup
    {
        public static void AddTacoServices(this IServiceCollection services) =>
            services.AddTacoServices<ProcessTacoStream>(null);

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public static void AddTacoServices<TReader>(this IServiceCollection services, Action<TacoConfiguration> options)
            where TReader:RestartableStream
        {
            var cfg = new TacoConfiguration()
            {
                DataSource = typeof(TReader).ToString()
            };
            options?.Invoke(cfg);
            services.AddSingleton(cfg);
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
