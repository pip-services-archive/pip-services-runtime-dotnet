namespace PipServices.Runtime.Errors
{
    public class ErrorCategory
    {
        public const string UnknownError = "InternalError";
        public const string BuildError = "BuildError";
        public const string ConfigError = "ConfigError";
        public const string StateError = "StateError";
        public const string ConnectionError = "ConnectionError";
        public const string CallError = "CallError";
        public const string FileError = "IOError";
        public const string BadRequest = "BadRequest";
        public const string Unauthorized = "Unauthorized";
        public const string NotFound = "NotFound";
        public const string Conflict = "Conflict";
        public const string Unsupported = "Unsupported";
    }
}