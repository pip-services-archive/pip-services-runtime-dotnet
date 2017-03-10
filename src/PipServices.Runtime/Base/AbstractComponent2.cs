using System;
using PipServices.Runtime.Config;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Base
{
    /// <summary>
    ///     Abstract class for all components.
    ///     In addition to mandatory component lifecycle management
    ///     it provides additional services like logging, performance counting,
    ///     cache, discovery, and others.
    /// </summary>
    public class AbstractComponent2 : IComponent
    {
        private ICache _cache;

        protected AbstractComponent2(ComponentDescriptor descriptor)
        {
            if (descriptor == null)
                throw new NullReferenceException("Descriptor is not set");

            Descriptor = descriptor;
            StateManager = new ComponentStateManager(this);
        }

        /// <summary>
        ///     Gets component configuration parameters.
        /// </summary>
        public ComponentConfig Config { get; private set; }

        /// <summary>
        ///     Gets component state manager.
        /// </summary>
        public ComponentStateManager StateManager { get; }

        /// <summary>
        ///     Gets component logging helper.
        /// </summary>
        public ComponentLogWriter Log { get; private set; }

        /// <summary>
        ///     Gets component performance counting helper.
        /// </summary>
        public ComponentCounters Counters { get; private set; }

        /// <summary>
        ///     Gets discovery service.
        /// </summary>
        public IDiscovery Discovery { get; private set; }

        /// <summary>
        ///     Gets the unique component descriptor that can identify
        ///     and locate the component inside the microservice.
        /// </summary>
        public ComponentDescriptor Descriptor { get; }

        /// <summary>
        ///     Gets the current state of the component.
        /// </summary>
        public State State
        {
            get { return StateManager.CurrentState; }
        }

        /// <summary>
        ///     Sets component configuration parameters and switches from component
        ///     to 'Configured' state.The configuration is only allowed once
        ///     right after creation.Attempts to perform reconfiguration will
        ///     cause an exception.
        /// </summary>
        /// <param name="config">the component configuration parameters.</param>
        public virtual void Configure(ComponentConfig config)
        {
            Config = config;
            StateManager.ChangeState(State.Configured);
        }

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
        /// <param name="components">References to microservice components.</param>
        public virtual void Link(DynamicMap context, ComponentSet components)
        {
            // Create helper objects
            Log = new ComponentLogWriter(this, components);
            Counters = new ComponentCounters(this, components);

            // Get references to component services
            Discovery = (IDiscovery) components.GetOneOptional(
                new ComponentDescriptor(Category.Discovery, null, null, null)
                );
            _cache = (ICache) components.GetOneOptional(
                new ComponentDescriptor(Category.Cache, null, null, null)
                );

            StateManager.ChangeState(State.Linked);
            Log.Trace(null, "Component " + Descriptor + " linked");
        }

        /// <summary>
        ///     Change state of component to Opening
        /// </summary>
        protected virtual void StartOpening()
        {
            StateManager.ChangeState(State.Opening);
            Log.Trace(null, "Component " + Descriptor + " opening");
        }

        /// <summary>
        ///     Opens the component, performs initialization, opens connections
        ///     to external services and makes the component ready for operations.
        ///     Opening can be done multiple times: right after linking
        ///     or reopening after closure.
        /// </summary>
        public virtual void Open()
        {
            StateManager.ChangeState(State.Opened);
            Log.Trace(null, "Component " + Descriptor + " opened");
        }

        /// <summary>
        ///     Closes the component and all open connections, performs deinitialization
        ///     steps.Closure can only be done from opened state.Attempts to close
        ///     already closed component or in wrong order will cause exception.
        /// </summary>
        public virtual void Close()
        {
            StateManager.ChangeState(State.Closed);
            Log.Trace(null, "Component " + Descriptor + " closed");
        }

        /// <summary>
        ///     Generates a string representation for this component
        /// </summary>
        /// <returns>a component descriptor in string format</returns>
        public string toString()
        {
            return Descriptor.ToString();
        }
    }
}