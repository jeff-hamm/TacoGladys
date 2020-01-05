using System;
using System.IO;
using System.Text.Json;

namespace TacoLib
{
    public class TacoConfigurationService 
    {
        private readonly TacoConfigurationOptions _config;

        public TacoConfigurationService(TacoConfigurationOptions config)
        {
            _config = config;
        }

        public TacoConfiguration Config => _config.Config;
        private string JsonConfigPath => System.IO.Path.Combine(System.AppContext.BaseDirectory, _config.ConfigFileName);

        public void WriteConfig()
        {
            WriteConfig(JsonConfigPath,Config);
        }

        internal static void WriteConfig(string jsonConfigPath, TacoConfiguration config)
        {

            var json = System.IO.File.ReadAllText(jsonConfigPath);
            using var writer = new Utf8JsonWriter(File.OpenWrite(jsonConfigPath));
            using var document = JsonDocument.Parse(json);
            writer.WriteStartObject();
            foreach (var element in document.RootElement.EnumerateObject())
            {
                if (!element.Name.Equals("TacoGladys", StringComparison.InvariantCultureIgnoreCase))
                {
                    element.WriteTo(writer);
                    continue;
                }

                writer.WritePropertyName("TacoGladys");
                JsonSerializer.Serialize(writer, config);
            }

            writer.WriteEndObject();
        }
    }
}