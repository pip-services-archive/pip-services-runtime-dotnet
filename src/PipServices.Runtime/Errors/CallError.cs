namespace PipServices.Runtime.Errors
{
    public class CallError : MicroserviceError
    {
        public CallError(string code, string message)
            : this(null, code, message)
        {
        }

        public CallError(IComponent component, string code, string message)
            : base(ErrorCategory.CallError, 500, component, code, message)
        {
        }
    }
}