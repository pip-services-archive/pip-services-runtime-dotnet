namespace PipServices.Runtime
{
    /// <summary>
    ///     Interface for microservice components that encapsulate
    ///     business logic.These components are the most valuable for
    ///     business and the key idea behind this framework is to protect
    ///     them from changes in persistence, communication or infrastructure
    ///     to ensure their long life.
    /// </summary>
    public interface IController : IBusinessLogic
    {
    }
}