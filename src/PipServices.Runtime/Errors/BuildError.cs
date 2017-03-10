namespace PipServices.Runtime.Errors
{
    public class BuildError : MicroserviceError
    {
        public BuildError(string code, string message)
            : this(null, code, message)
        {
        }

        public BuildError(IComponent component, string code, string message)
            : base(ErrorCategory.BuildError, 500, component, code, message)
        {
        }
    }
}