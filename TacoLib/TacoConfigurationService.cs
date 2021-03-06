﻿using System;
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
            _config.Config = ReadConfig(JsonConfigPath);
        }

        public TacoConfiguration Config => _config.Config;
        private string JsonConfigPath => System.IO.Path.Combine(System.AppContext.BaseDirectory, _config.ConfigFileName);

        public void WriteConfig(TacoConfiguration config)
        {
            _config.Config = config;
            WriteConfig(JsonConfigPath,_config.Config);
        }
        public void WriteConfig()
        {
            WriteConfig(JsonConfigPath,Config);
        }

        private readonly static JsonWriterOptions ConfigWriterOptions = new JsonWriterOptions()
        {
            Indented = true
        };
        private readonly static JsonSerializerOptions ConfigSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        private class TacoConfigurationFile
        {
            public TacoConfiguration TacoGladys { get; set; }
        }

        internal static TacoConfiguration ReadConfig(string jsonConfigPath) =>
            JsonSerializer.Deserialize<TacoConfigurationFile>(System.IO.File.ReadAllText(jsonConfigPath)).TacoGladys;

        internal static void WriteConfig(string jsonConfigPath, TacoConfiguration config)
        {

            var json = System.IO.File.ReadAllText(jsonConfigPath);
            using var document = JsonDocument.Parse(json);
            try
            {
                using var writeStream = new FileStream(jsonConfigPath, FileMode.Create);
                using var writer = new Utf8JsonWriter(writeStream, ConfigWriterOptions);
                writer.WriteStartObject();
                foreach (var element in document.RootElement.EnumerateObject())
                {
                    if (!element.Name.Equals("TacoGladys", StringComparison.InvariantCultureIgnoreCase))
                    {
                        element.WriteTo(writer);
                        continue;
                    }

                    writer.WritePropertyName("TacoGladys");
                    JsonSerializer.Serialize(writer, config, ConfigSerializerOptions);
                }

                writer.WriteEndObject();
            }
            catch
            {
                File.WriteAllText(jsonConfigPath,json);
                throw;
            }
        }
    }
}