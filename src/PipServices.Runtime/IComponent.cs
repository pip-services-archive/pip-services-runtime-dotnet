using PipServices.Runtime.Config;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime
{
    /// <summary>
    ///     The most basic interface that identifies microservice component
    ///     and it's behavior. It exposes unique component descriptor for
    ///     identification and allows to manage the component lifecycle to
    ///     transition between several states:
    ///     <ul>
    ///         <li>
    ///             Create - creates a new component instance
    ///             <li>
    ///                 Configure - sets component configuration parameters
    ///                 <li>
    ///                     Link - sets references to other microservice components
    ///                     <li>
    ///                         Open - performs initialization, opens connections and makes the component ready
    ///                         <li> Close - closes connections, deinitializes component.
    ///     </ul>
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        ///     Gets the unique component descriptor that can identify
        ///     and locate the component inside the microservice.
        /// </summary>
        ComponentDescriptor Descriptor { get; }

        /// <summary>
        ///     Gets the current state of the component.
        /// </summary>
        State State { get; }

        /// <summary>
        ///     Sets component configuration parameters and switches from component
        ///     to 'Configured' state.The configuration is only allowed once
        ///     right after creation.Attempts to perform reconfiguration will
        ///     cause an exception.
        /// </summary>
        /// <param name="config">the component configuration parameters.</param>
        void Configure(ComponentConfig config);

        /// <summary>
        ///     Sets references to other microservice components to enable their
        ///     collaboration.It is recommended to locate necessary components
        ///     and cache their references to void performance hit during
        ///     normal operations.
        ///     Linking can only be performed once after configuration
        ///     and will cause an exception when it is called second time
        ///     or out of order.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="components">references to microservice components.</param>
        void Link(DynamicMap context, ComponentSet components);

        /// <summary>
        ///     Opens the component, performs initialization, opens connections
        ///     to external services and makes the component ready for operations.
        ///     Opening can be done multiple times: right after linking
        ///     or reopening after closure.
        /// </summary>
        void Open();

        /// <summary>
        ///     Closes the component and all open connections, performs deinitialization
        ///     steps.Closure can only be done from opened state.Attempts to close
        ///     already closed component or in wrong order will cause exception.
        /// </summary>
        void Close();
    }
}