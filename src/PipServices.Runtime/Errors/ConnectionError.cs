namespace PipServices.Runtime.Errors
{
    public class ConnectionError : MicroserviceError
    {
        public ConnectionError(string code, string message)
            : this(null, code, message)
        {
        }

        public ConnectionError(IComponent component, string code, string message)
            : base(ErrorCategory.ConnectionError, 500, component, code, message)
        {
        }
    }
}