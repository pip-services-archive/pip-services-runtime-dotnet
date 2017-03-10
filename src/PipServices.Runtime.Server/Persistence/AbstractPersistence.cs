using PipServices.Runtime.Config;
using PipServices.Runtime.Data;

namespace PipServices.Runtime.Persistence
{
    /// <summary>
    ///     Abstract implementation of microservice persistence components
    ///     that store and retrieve persistent data.
    /// </summary>
    public abstract class AbstractPersistence : AbstractComponent, IPersistence
    {
        /// <summary>
        ///     Creates instance of abstract persistence component.
        /// </summary>
        /// <param name="descriptor">the unique descriptor that is used to identify and locate the component.</param>
        protected AbstractPersistence(ComponentDescriptor descriptor)
            : base(descriptor)
        {
        }

        /// <summary>
        ///     Generates globally unique string GUID to identify stored object.
        ///     Usage of string GUIDs for object ids is one of the key Pip.Services
        ///     patterns that helps to ensure portability across all persistence storages
        ///     and language implementations.
        /// </summary>
        /// <returns>a globally unique GUID</returns>
        protected string CreateUuid()
        {
            return IdGenerator.Uuid();
        }

        /// <summary>
        ///     Clears persistence storage. This method shall only be used in testing
        ///     and shall never be called in production.
        /// </summary>
        public virtual void Clear()
        {
        }
    }
}