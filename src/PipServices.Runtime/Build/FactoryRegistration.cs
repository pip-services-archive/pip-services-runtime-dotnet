using System;
using PipServices.Runtime.Config;

namespace PipServices.Runtime.Build
{
    /// <summary>
    ///     Holds registration of specific component in component factory.
    /// </summary>
    internal class FactoryRegistration
    {
        /// <summary>
        ///     Creates and fills registration instance.
        /// </summary>
        /// <param name="descriptor">a component descriptor to locate the registration</param>
        /// <param name="classFactory">a component class factory to instantiate a component</param>
        public FactoryRegistration(ComponentDescriptor descriptor, Type classFactory)
        {
            Descriptor = descriptor;
            ClassFactory = classFactory;
        }

        /// <summary>
        ///     Gets a component descriptor for matching
        /// </summary>
        public ComponentDescriptor Descriptor { get; }

        /// <summary>
        ///     Get a component type factory to create a component instance
        /// </summary>
        public Type ClassFactory { get; }
    }
}