namespace PipServices.Runtime.Errors
{
    public class StateError : MicroserviceError
    {
        public StateError(string code, string message)
            : this(null, code, message)
        {
        }

        public StateError(IComponent component, string code, string message)
            : base(ErrorCategory.StateError, 500, component, code, message)
        {
        }
    }
}