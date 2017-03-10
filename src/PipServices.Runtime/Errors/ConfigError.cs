namespace PipServices.Runtime.Errors
{
    public class ConfigError : MicroserviceError
    {
        public ConfigError(string code, string message)
            : this(null, code, message)
        {
        }

        public ConfigError(IComponent component, string code, string message)
            : base(ErrorCategory.ConfigError, 500, component, code, message)
        {
        }
    }
}