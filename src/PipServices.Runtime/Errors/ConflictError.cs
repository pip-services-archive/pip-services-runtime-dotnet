namespace PipServices.Runtime.Errors
{
    public class ConflictError : MicroserviceError
    {
        public ConflictError(string code, string message)
            : this(null, code, message)
        {
        }

        public ConflictError(IComponent component, string code, string message)
            : base(ErrorCategory.Conflict, 409, component, code, message)
        {
        }
    }
}