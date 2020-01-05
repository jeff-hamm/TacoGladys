using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TacoLib;
using TacoServer.Hubs;

namespace TacoServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddKendo();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
            services.AddOptions();
            services.Configure<TacoConfiguration>(Configuration.GetSection("TacoGladys"));
            services.AddTacoServices(options =>
            {
                options.ConfigFileName = "tacogladys.json";
            });
            services.AddSignalR()
                .AddHubOptions((HubOptions<Hub> o) => { o.EnableDetailedErrors = true; })
//                .AddHubOptions(c => c.EnableDetailedErrors = true)
//                .AddMessagePackProtocol();
;
            services.AddLogging(logging =>
            {
//                logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                logging.AddConsole().AddDebug().AddEventSourceLogger().SetMinimumLevel(LogLevel.Trace);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
//            app.UseClientSideBlazorFiles<global::RPiBlazor.Client.Startup>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
//                endpoints.MapDefaultControllerRoute();
//                endpoints.MapFallbackToClientSideBlazor<global::RPiBlazor.Client.Startup>("index.html");
                endpoints.MapHub<TacoGladysHub>("/tacoStream");
            });
        }
    }
}
