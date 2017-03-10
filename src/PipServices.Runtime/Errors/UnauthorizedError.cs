namespace PipServices.Runtime.Errors
{
    public class UnauthorizedError : MicroserviceError
    {
        public UnauthorizedError(string code, string message)
            : this(null, code, message)
        {
        }

        public UnauthorizedError(IComponent component, string code, string message)
            : base(ErrorCategory.Unauthorized, 401, component, code, message)
        {
        }
    }
}