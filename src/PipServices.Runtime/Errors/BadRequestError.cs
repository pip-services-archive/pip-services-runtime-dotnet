namespace PipServices.Runtime.Errors
{
    public class BadRequestError : MicroserviceError
    {
        public BadRequestError(string code, string message)
            : this(null, code, message)
        {
        }

        public BadRequestError(IComponent component, string code, string message)
            : base(ErrorCategory.BadRequest, 400, component, code, message)
        {
        }
    }
}