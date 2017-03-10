namespace PipServices.Runtime
{
    /// <summary>
    ///     Decorators are used to inject custom behavior into
    ///     existing microservice.They alter business logic before or after
    ///     execution or may override it entirely.
    ///     The custom logic can make use of custom fields
    ///     in persisted data or may call custom services.
    /// </summary>
    public interface IDecorator : IBusinessLogic
    {
    }
}