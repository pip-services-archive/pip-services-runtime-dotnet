using System;
using System.Collections.Generic;
using PipServices.Runtime.Config;
using PipServices.Runtime.Errors;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Build
{
    /// <summary>
    ///     Factory for microservice components. It registers component classes,
    ///     locates classes by descriptors and creates component instances.
    ///     It also supports inheritance from other factories.
    /// </summary>
    public class ComponentFactory : IComponentFactory
    {
        private readonly List<IComponentFactory> _baseFactories = new List<IComponentFactory>();
        private readonly List<FactoryRegistration> _registrations = new List<FactoryRegistration>();

        /// <summary>
        ///     Creates an instance of component factory.
        /// </summary>
        public ComponentFactory()
        {
        }

        /// <summary>
        ///     Creates an instance of component factory and extends it with base factories.
        /// </summary>
        /// <param name="baseFactories">base factories to extend registrations of this factory.</param>
        public ComponentFactory(params IComponentFactory[] baseFactories)
        {
            _baseFactories.AddRange(baseFactories);
        }

        /// <summary>
        ///     Extends this factory with base factories.
        /// </summary>
        /// <param name="baseFactories">a list of base factories to extend registrations of this factory.</param>
        public void Extend(params IComponentFactory[] baseFactories)
        {
            _baseFactories.AddRange(baseFactories);
        }

        /// <summary>
        ///     Registers a component class accompanies by component descriptor.
        /// </summary>
        /// <param name="descriptor">a component descriptor to locate the class.</param>
        /// <param name="classFactory">a component class used to create a component.</param>
        /// <returns>a reference to this factory to support chaining registrations.</returns>
        public IComponentFactory Register(ComponentDescriptor descriptor, Type classFactory)
        {
            if (descriptor == null)
                throw new NullReferenceException("Descriptor cannot be null");
            if (classFactory == null)
                throw new NullReferenceException("Class factory cannot be null");

            _registrations.Add(new FactoryRegistration(descriptor, classFactory));
            return this;
        }

        /// <summary>
        ///     Lookups for component class by matching component descriptor.
        /// </summary>
        /// <param name="descriptor">a component descriptor used to locate a class</param>
        /// <returns>a located component class.</returns>
        public Type Find(ComponentDescriptor descriptor)
        {
            // Try to find a match in local registrations
            foreach (var registration in _registrations)
            {
                if (registration.Descriptor.Match(descriptor))
                    return registration.ClassFactory;
            }

            foreach (var baseFactory in _baseFactories)
            {
                var classFactory = baseFactory.Find(descriptor);
                if (classFactory != null)
                    return classFactory;
            }

            return null;
        }

        /// <summary>
        ///     Create a component instance using class located by component descriptor.
        /// </summary>
        /// <param name="descriptor">a component descriptor to locate a component class.</param>
        /// <returns>a created component instance.</returns>
        public IComponent Create(ComponentDescriptor descriptor)
        {
            object component;

            try
            {
                // Create a component
                var classFactory = Find(descriptor);

                if (classFactory == null)
                {
                    throw new ConfigError(
                        "FactoryNotFound",
                        "Factory for component " + descriptor + " was not found"
                        ).WithDetails(descriptor);
                }

                component = Activator.CreateInstance(classFactory);
            }
            catch (Exception ex)
            {
                throw new BuildError(
                    "CreateFailed",
                    "Failed to instantiate component " + descriptor + ": " + ex
                    )
                    .WithDetails(descriptor)
                    .Wrap(ex);
            }

            if (!(component is IComponent))
                throw new BuildError(
                    "BadComponent",
                    "Component " + descriptor + " does not implement IComponent interface"
                    ).WithDetails(descriptor);

            return (IComponent) component;
        }

        /// <summary>
        ///     Dynamically creates an instance of configuration factory based
        ///     on configuration parameters.
        /// </summary>
        /// <param name="config">a configuration parameters to locate the factory class.</param>
        /// <returns>a created factory instance</returns>
        public static IComponentFactory CreateFactory(DynamicMap config)
        {
            //// Shortcut if class if specified as a class function
            //Type type = config.Get("class") as Type;
            //if (type != null)
            //    return Activator.CreateInstance(type, config) as IComponent;

            //// Get and check class/type name
            //string typeName = config.GetNullableString("class");
            //if (typeName == null)
            //    throw new ConfigError("NoType", id + " custom class is not configured").WithDetails(typeName);

            //// Get and check component module name
            //string moduleName = config.GetNullableString("assembly")
            //    ?? config.GetNullableString("module")
            //    ?? config.GetNullableString("library");

            //Assembly assembly = moduleName != null ? Assembly.LoadFrom(moduleName) : Assembly.GetExecutingAssembly();
            //if (assembly == null)
            //    throw new ConfigError("NoAssembly", id + " assembly is incorrect").WithDetails(moduleName);

            //type = assembly.GetType(typeName);
            //if (type == null)
            //    throw new ConfigError("NoType", id + " type name is incorrect").WithDetails(typeName);

            //// Create a component
            //return Activator.CreateInstance(type, config) as IComponent;

            return null;
        }
    }
}