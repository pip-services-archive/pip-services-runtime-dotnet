using System.IO;
using PipServices.Runtime.Config;
using PipServices.Runtime.Errors;

namespace PipServices.Runtime.Boot
{
    public class FileBootConfig : AbstractBootConfig
    {
        /// <summary>
        ///     Unique descriptor for the FileConfigReader component
        /// </summary>
        public static readonly ComponentDescriptor ClassDescriptor = new ComponentDescriptor(
            Category.Boot, "pip-services-runtime-boot", "file", "*"
            );

        private string _path;

        public FileBootConfig()
            : base(ClassDescriptor)
        {
        }

        public override void Configure(ComponentConfig config)
        {
            CheckNewStateAllowed(State.Configured);

            var options = config.Options;
            if (options == null || options.HasNot("path"))
                throw new ConfigError(this, "NoPath", "Missing config file path");

            base.Configure(config);

            _path = options.GetString("path");
        }

        public override void Open()
        {
            StartOpening();

            CheckNewStateAllowed(State.Opened);

            if (!File.Exists(_path))
            {
                throw new NotFoundError(
                    this,
                    "FileNotFound",
                    "Config file was not found at " + _path
                    ).WithDetails(_path);
            }

            base.Open();
        }

        public override MicroserviceConfig ReadConfig()
        {
            return ConfigReader.Read(_path);
        }
    }
}