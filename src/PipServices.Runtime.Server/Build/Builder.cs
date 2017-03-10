using System;
using System.Collections.Generic;
using System.Reflection;
using PipServices.Runtime.Config;
using PipServices.Runtime.Errors;

namespace PipServices.Runtime.Build
{
    /// <summary>
    ///     Builds microservice components using configuration as a build recipe.
    /// </summary>
    public class Builder
    {
        public static object CreateDynamicObject(ComponentConstructor constructor)
        {
            try
            {
                // Create a component
                Type classFactory;

                if (constructor.Class == null)
                {
                    throw new ConfigError(
                        "NoClass",
                        "Class in component constructor is not defined"
                        );
                }

                if (constructor.Assembly != null)
                {
                    var assembly = Assembly.LoadFrom(constructor.Assembly);
                    classFactory = assembly.GetType(constructor.Class);
                }
                else
                {
                    classFactory = Type.GetType(constructor.Class);
                }

                if (classFactory == null)
                {
                    throw new ConfigError(
                        "FactoryNotFound",
                        "Factory for component " + constructor + " was not found"
                        ).WithDetails(constructor);
                }

                return Activator.CreateInstance(classFactory);
            }
            catch (Exception ex)
            {
                throw new BuildError(
                    "CreateFailed",
                    "Failed to instantiate component " + constructor + ": " + ex
                    )
                    .WithDetails(constructor)
                    .Wrap(ex);
            }
        }

        public static IComponentFactory CreateDynamicFactory(ComponentConstructor constructor)
        {
            var component = CreateDynamicObject(constructor);

            if (!(component is IComponentFactory))
                throw new BuildError(
                    "BadComponentFactory",
                    "Component " + constructor + " does not implement IComponentFactory interface"
                    ).WithDetails(constructor);

            return (IComponentFactory) component;
        }

        /// <summary>
        ///     Loads dynamic factories and adds them to the base factory
        /// </summary>
        /// <param name="factory">a component factory used by the builder.</param>
        /// <param name="config">a microservice configuration</param>
        public static void LoadFactories(IComponentFactory factory, MicroserviceConfig config)
        {
            // Get specified configuration section
            var componentConfigs = config.GetSection(Category.Factories);

            // Go through configured components one by one
            foreach (var componentConfig in componentConfigs)
            {
                // Create factories dynamically
                var descriptor = componentConfig.Descriptor;
                var constructor = componentConfig.Constructor;
                if (constructor == null)
                    throw new ConfigError(
                        "NoConstructor",
                        "Dynamic factory " + descriptor + " has no constructor defined"
                        ).WithDetails(descriptor);

                var dynamicFactory = CreateDynamicFactory(constructor);

                // Add dynamic factory to the builder factory
                factory.Extend(dynamicFactory);
            }
        }

        /// <summary>
        ///     Builds default components for specified configuration section.
        /// </summary>
        /// <param name="factory">a component factory that creates component instances.</param>
        /// <param name="category">a name of the section inside configuration.</param>
        /// <param name="components">a list with section components</param>
        /// <returns>a list with section components for chaining</returns>
        private static List<IComponent> BuildSectionDefaults(
            IComponentFactory factory, string category, List<IComponent> components)
        {
            // Todo: Add null discovery by default
            if (category.Equals(Category.Discovery) && components.Count == 0)
            {
            }
            // Add null log by default
            else if (category.Equals(Category.Logs) && components.Count == 0)
            {
                var log = factory.Create(new ComponentDescriptor(Category.Logs, null, "null", null));
                log.Configure(new ComponentConfig());
                components.Add(log);
            }
            // Add null counters by default
            else if (category.Equals(Category.Counters) && components.Count == 0)
            {
                var counters = factory.Create(new ComponentDescriptor(Category.Counters, null, "null", null));
                counters.Configure(new ComponentConfig());
                components.Add(counters);
            }
            // Add null cache by default
            else if (category.Equals(Category.Cache) && components.Count == 0)
            {
                var cache = factory.Create(new ComponentDescriptor(Category.Cache, null, "null", null));
                cache.Configure(new ComponentConfig());
                components.Add(cache);
            }
            return components;
        }

        public static IComponent CreateDynamicComponent(ComponentConstructor constructor)
        {
            var component = CreateDynamicObject(constructor);

            if (!(component is IComponent))
                throw new BuildError(
                    "BadComponent",
                    "Component " + constructor + " does not implement IComponent interface"
                    ).WithDetails(constructor);

            return (IComponent) component;
        }

        /// <summary>
        ///     Builds components from specific configuration section.
        /// </summary>
        /// <param name="factory">a component factory that creates component instances.</param>
        /// <param name="config">a microservice configuration</param>
        /// <param name="category">a name of the section inside configuration.</param>
        /// <returns>a list with created components</returns>
        public static List<IComponent> BuildSection(
            IComponentFactory factory, MicroserviceConfig config, string category)
        {
            var components = new List<IComponent>();

            // Get specified configuration section
            var componentConfigs = config.GetSection(category);

            // Go through configured components one by one
            foreach (var componentConfig in componentConfigs)
            {
                // Create component statically or dynamically
                var descriptor = componentConfig.Descriptor;
                var constructor = componentConfig.Constructor;
                var component = constructor != null
                    ? CreateDynamicComponent(constructor)
                    : factory.Create(descriptor);

                // Configure the created component
                component.Configure(componentConfig);
                components.Add(component);
            }

            // Add default components and return the result
            return BuildSectionDefaults(factory, category, components);
        }

        public static ComponentSet Build(IComponentFactory baseFactory, MicroserviceConfig config)
        {
            if (baseFactory == null)
                throw new NullReferenceException("Factory isn't set");
            if (config == null)
                throw new NullReferenceException("Microservice config isn't set");

            // Loading dynamic factories
            var factory = new ComponentFactory(baseFactory);
            LoadFactories(factory, config);

            // Create components section by section
            var components = new ComponentSet();
            components.AddAll(BuildSection(factory, config, Category.Discovery));
            components.AddAll(BuildSection(factory, config, Category.Logs));
            components.AddAll(BuildSection(factory, config, Category.Counters));
            components.AddAll(BuildSection(factory, config, Category.Cache));
            components.AddAll(BuildSection(factory, config, Category.Clients));
            components.AddAll(BuildSection(factory, config, Category.Persistence));
            components.AddAll(BuildSection(factory, config, Category.Controllers));
            components.AddAll(BuildSection(factory, config, Category.Decorators));
            components.AddAll(BuildSection(factory, config, Category.Services));
            components.AddAll(BuildSection(factory, config, Category.Addons));
            return components;
        }
    }
}