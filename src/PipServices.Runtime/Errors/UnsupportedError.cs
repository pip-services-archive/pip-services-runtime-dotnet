namespace PipServices.Runtime.Errors
{
    public class UnsupportedError : MicroserviceError
    {
        public UnsupportedError(string code, string message)
            : this(null, code, message)
        {
        }

        public UnsupportedError(IComponent component, string code, string message)
            : base(ErrorCategory.Unsupported, 500, component, code, message)
        {
        }
    }
}