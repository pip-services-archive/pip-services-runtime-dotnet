using System;
using PipServices.Runtime.Config;

namespace PipServices.Runtime
{
    /// <summary>
    ///     Factory for microservice components. It registers component classes,
    ///     locates classes by descriptors and creates component instances.
    ///     It also supports inheritance from other factories.
    /// </summary>
    public interface IComponentFactory
    {
        /// <summary>
        ///     Extends this factory with base factories.
        /// </summary>
        /// <param name="baseFactories">a list of base factories to extend registrations of this factory.</param>
        void Extend(params IComponentFactory[] baseFactories);

        /// <summary>
        ///     Registers a component class accompanies by component descriptor.
        /// </summary>
        /// <param name="descriptor">a component descriptor to locate the class.</param>
        /// <param name="classFactory">a component class used to create a component.</param>
        /// <returns>a reference to this factory to support chaining registrations.</returns>
        IComponentFactory Register(ComponentDescriptor descriptor, Type classFactory);

        /// <summary>
        ///     Lookups for component class by matching component descriptor.
        /// </summary>
        /// <param name="descriptor">a component descriptor used to locate a class</param>
        /// <returns>a located component class.</returns>
        Type Find(ComponentDescriptor descriptor);

        /// <summary>
        ///     Create a component instance using class located by component descriptor.
        /// </summary>
        /// <param name="descriptor">a component descriptor to locate a component class.</param>
        /// <returns>a created component instance.</returns>
        IComponent Create(ComponentDescriptor descriptor);
    }
}