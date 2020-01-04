using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TacoLib.Data;
using TacoLib.Interop;

namespace TacoLib.Tests
{
    public class TestBase
    {
        protected IServiceProvider ConfigureAndBuild()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                //               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            IServiceCollection services = new ServiceCollection();
            services.AddTacoServices();

            return services.BuildServiceProvider();
        }

        protected Stream OpenDemoStream() =>  System.IO.File.OpenRead("./data/demolog.bin");

        protected ITacoMessageParser GetFormatter() => Provider.GetService<AldlIoMessageParser>();

        private IServiceProvider _provider;

        protected IServiceProvider Provider
        {
            get => _provider ??= ConfigureAndBuild();
            set => _provider = value;
        }
/*        protected async Task<TacoDefinition[]> ReadDefinitions(ITacoMessageParser parser)
        {
            using (var stream = OpenDemoStream())
            {
                await parser.ParseAldlDefinitions(stream);
                return parser.Defintitions;
            }
        }
        protected async Task<TacoMessage> ReadMessage(ITacoMessageParser parser)
        {
            using (var stream = OpenDemoStream())
            {
                await parser.ParseAldlDefinitions(stream);
                var buffer = new TacoMessageBuffer()
                {
                    Data = new byte[parser.BufferSize],
                    DataLength = parser.BufferSize,
                    StartTime = DateTime.Now,
                    LastUpdated = DateTime.Now.Ticks
                };
                buffer.DataLength = stream.Read(buffer.Data, 0, parser.BufferSize);
                return parser.ParseMessage(buffer, DateTime.Now.Ticks);
            }
        }*/
    }
}