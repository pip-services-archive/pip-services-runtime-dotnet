namespace PipServices.Runtime.Errors
{
    public class FileError : MicroserviceError
    {
        public FileError(string code, string message)
            : this(null, code, message)
        {
        }

        public FileError(IComponent component, string code, string message)
            : base(ErrorCategory.FileError, 500, component, code, message)
        {
        }
    }
}