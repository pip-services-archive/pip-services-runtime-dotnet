namespace PipServices.Runtime.Errors
{
    public class UnknownError : MicroserviceError
    {
        public UnknownError(string code, string message)
            : this(null, code, message)
        {
        }

        public UnknownError(IComponent component, string code, string message)
            : base(ErrorCategory.UnknownError, 500, component, code, message)
        {
        }
    }
}