namespace PipServices.Runtime.Errors
{
    public class NotFoundError : MicroserviceError
    {
        public NotFoundError(string code, string message)
            : this(null, code, message)
        {
        }

        public NotFoundError(IComponent component, string code, string message)
            : base(ErrorCategory.NotFound, 404, component, code, message)
        {
        }
    }
}