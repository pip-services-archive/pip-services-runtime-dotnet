using System;
using System.IO;
using Newtonsoft.Json;
using PipServices.Runtime.Errors;
using YamlDotNet.Serialization;

namespace PipServices.Runtime.Config
{
    public static class ConfigReader
    {
        private static readonly Deserializer Deserializer = new Deserializer();

        /// <summary>
        ///     Reads microservice configuration from file.
        ///     File type is automatically determined by extension.
        /// </summary>
        /// <param name="path">a path to configuration file.</param>
        public static MicroserviceConfig Read(string path)
        {
            if (path == null)
                throw new ConfigError("NoPath", "Missing config file path");

            var index = path.LastIndexOf('.');
            var ext = index > 0 ? path.Substring(index + 1).ToLower() : "";

            if (ext == "json")
                return ReadJson(path);

            if (ext == "yaml")
                return ReadYaml(path);

            // By default read files as JSON
            return ReadJson(path);
        }

        /// <summary>
        ///     Reads microservice configuration from JSON file on disk.
        /// </summary>
        /// <param name="path">a path to configuration file.</param>
        public static MicroserviceConfig ReadJson(string path)
        {
            if (path == null)
                throw new ConfigError("NoPath", "Missing config file path");

            try
            {
                using (var reader = new StreamReader(path))
                {
                    var json = reader.ReadToEnd();
                    object config = JsonConvert.DeserializeObject<dynamic>(json);
                    return MicroserviceConfig.FromValue(config);
                }
            }
            catch (Exception ex)
            {
                throw new FileError(
                    "ReadFiled",
                    "Failed reading configuration " + path + ": " + ex
                    )
                    .WithDetails(path)
                    .Wrap(ex);
            }
        }

        /// <summary>
        ///     Reads microservice configuration from YAML file on disk.
        /// </summary>
        /// <param name="path">a path to configuration file.</param>
        public static MicroserviceConfig ReadYaml(string path)
        {
            if (path == null)
                throw new ConfigError("NoPath", "Missing config file path");

            try
            {
                using (var reader = new StreamReader(path))
                {
                    var config = Deserializer.Deserialize(reader);
                    return MicroserviceConfig.FromValue(config);
                }
            }
            catch (Exception ex)
            {
                throw new FileError(
                    "ReadFiled",
                    "Failed reading configuration " + path + ": " + ex
                    )
                    .WithDetails(path)
                    .Wrap(ex);
            }
        }
    }
}