using System;
using System.Collections.Generic;
using PipServices.Runtime.Config;
using PipServices.Runtime.Errors;

namespace PipServices.Runtime
{
    /// <summary>
    ///     A list with references to all microservice components.
    ///     It is capable of searching and retrieving components
    ///     by specified criteria.
    /// </summary>
    public class ComponentSet
    {
        private readonly List<IComponent> _components = new List<IComponent>();

        /// <summary>
        ///     reates an empty component list
        /// </summary>
        public ComponentSet()
        {
        }

        /// <summary>
        ///     Creates a component list and fills with component references from another list.
        /// </summary>
        /// <param name="components">a list of components to add to this list.</param>
        public ComponentSet(IEnumerable<IComponent> components)
        {
            _components.AddRange(components);
        }

        /// <summary>
        ///     Adds a single component to the list
        /// </summary>
        /// <param name="component">a component to be added to the list</param>
        public void Add(IComponent component)
        {
            _components.Add(component);
        }

        /// <summary>
        ///     Adds multiple components to the list
        /// </summary>
        /// <param name="components">a list of components to be added.</param>
        public void AddAll(IEnumerable<IComponent> components)
        {
            _components.AddRange(components);
        }

        /// <summary>
        ///     Internal utility method to fill a list with components from a specific category.
        /// </summary>
        /// <param name="components">a component list where found components shall be added</param>
        /// <param name="category">a category to pick components.</param>
        /// <returns>a reference to the component list for chaining.</returns>
        private List<IComponent> AddByCategory(List<IComponent> components, string category)
        {
            foreach (var component in _components)
            {
                if (component.Descriptor.Category.Equals(category))
                    components.Add(component);
            }
            return components;
        }

        /// <summary>
        ///     Gets a sublist of component references from specific category.
        /// </summary>
        /// <param name="category">a category to pick components.</param>
        /// <returns>a list of found components</returns>
        public IList<IComponent> GetAllByCategory(string category)
        {
            return AddByCategory(new List<IComponent>(), category);
        }

        /// <summary>
        ///     Get a list of components in random order.
        ///     Since it doesn't perform additional calculations
        ///     this operation is faster then getting ordered list.
        /// </summary>
        /// <returns>an unsorted list of components.</returns>
        public IList<IComponent> GetAllUnordered()
        {
            return _components;
        }

        /// <summary>
        ///     Gets a list with all component references sorted in strict
        ///     initialization order: Discovery, Logs, Counters, Cache, Persistence, Controller, ...
        ///     This sorting order it require to lifecycle management to proper sequencing.
        /// </summary>
        /// <returns>a sorted list of components</returns>
        public IList<IComponent> GetAllOrdered()
        {
            var result = new List<IComponent>();
            AddByCategory(result, Category.Discovery);
            AddByCategory(result, Category.Boot);
            AddByCategory(result, Category.Logs);
            AddByCategory(result, Category.Counters);
            AddByCategory(result, Category.Cache);
            AddByCategory(result, Category.Persistence);
            AddByCategory(result, Category.Clients);
            AddByCategory(result, Category.Controllers);
            AddByCategory(result, Category.Decorators);
            AddByCategory(result, Category.Services);
            AddByCategory(result, Category.Addons);
            return result;
        }

        /// <summary>
        ///     Finds all components that match specified descriptor.
        ///     The descriptor is used to specify number of search criteria
        ///     or their combinations:
        ///     <ul>
        ///         <li>
        ///             By category
        ///             <li>
        ///                 By logical group
        ///                 <li>
        ///                     By functional type
        ///                     <li> By implementation version
        ///     </ul>
        /// </summary>
        /// <param name="descriptor">a component descriptor as a search criteria</param>
        /// <returns>a list with found components</returns>
        public IList<IComponent> GetAllOptional(ComponentDescriptor descriptor)
        {
            if (descriptor == null)
                throw new NullReferenceException("Descriptor is not set");

            var result = new List<IComponent>();
            // Search from the end to account for decorators
            for (var i = _components.Count - 1; i >= 0; i--)
            {
                var component = _components[i];
                if (component.Descriptor.Match(descriptor))
                    result.Add(component);
            }
            return result;
        }

        /// <summary>
        ///     Finds the a single component instance (the first one)
        ///     that matches to the specified descriptor.
        ///     The descriptor is used to specify number of search criteria
        ///     or their combinations:
        ///     <ul>
        ///         <li>
        ///             By category
        ///             <li>
        ///                 By logical group
        ///                 <li>
        ///                     By functional type
        ///                     <li> By implementation version
        ///     </ul>
        /// </summary>
        /// <param name="descriptor">a component descriptor as a search criteria</param>
        /// <returns>a found component instance or <b>null</b> if nothing was found.</returns>
        public IComponent GetOneOptional(ComponentDescriptor descriptor)
        {
            if (descriptor == null)
                throw new NullReferenceException("Descriptor is not set");

            // Search from the end to account for decorators
            for (var i = _components.Count - 1; i >= 0; i--)
            {
                var component = _components[i];
                if (component.Descriptor.Match(descriptor))
                    return component;
            }
            return null;
        }

        /// <summary>
        ///     Gets all components that match specified descriptor.
        ///     If no components found it throws a configuration error.
        ///     The descriptor is used to specify number of search criteria
        ///     or their combinations:
        ///     <ul>
        ///         <li>
        ///             By category
        ///             <li>
        ///                 By logical group
        ///                 <li>
        ///                     By functional type
        ///                     <li> By implementation version
        ///     </ul>
        /// </summary>
        /// <param name="descriptor">a component descriptor as a search criteria</param>
        /// <returns>a list with found components</returns>
        public IList<IComponent> GetAllRequired(ComponentDescriptor descriptor)
        {
            if (descriptor == null)
                throw new NullReferenceException("Descriptor is not set");

            var result = GetAllOptional(descriptor);
            if (result.Count == 0)
            {
                throw new ConfigError(
                    "NoDependency",
                    "At least one component " + descriptor + " must be present to satisfy dependencies"
                    ).WithDetails(descriptor);
            }
            return result;
        }

        /// <summary>
        ///     Gets a component instance that matches the specified descriptor.
        ///     If nothing is found it throws a configuration error.
        ///     The descriptor is used to specify number of search criteria
        ///     or their combinations:
        ///     <ul>
        ///         <li>
        ///             By category
        ///             <li>
        ///                 By logical group
        ///                 <li>
        ///                     By functional type
        ///                     <li> By implementation version
        ///     </ul>
        /// </summary>
        /// <param name="descriptor">a component descriptor as a search criteria</param>
        /// <returns>a found component instance</returns>
        public IComponent GetOneRequired(ComponentDescriptor descriptor)
        {
            if (descriptor == null)
                throw new NullReferenceException("Descriptor is not set");

            var result = GetOneOptional(descriptor);
            if (result == null)
            {
                throw new ConfigError(
                    "NoDependency",
                    "Component " + descriptor + " must present to satisfy dependencies"
                    ).WithDetails(descriptor);
            }
            return result;
        }

        /// <summary>
        ///     Gets a component instance that matches the specified descriptor defined
        ///     <b>before</b> specified instance.If nothing is found it throws a configuration error.
        ///     This method is used primarily to find dependencies between business logic components
        ///     in their logical chain.The sequence goes in order as components were configured.
        ///     The descriptor is used to specify number of search criteria
        ///     or their combinations:
        ///     <ul>
        ///         <li>
        ///             By category
        ///             <li>
        ///                 By logical group
        ///                 <li>
        ///                     By functional type
        ///                     <li> By implementation version
        ///     </ul>
        ///     For instance, quite often the descriptor will look as "logic / group / * / *"
        /// </summary>
        /// <param name="component">a component that searches for prior dependencies</param>
        /// <param name="descriptor">a component descriptor as a search criteria</param>
        /// <returns>a found component instance</returns>
        public IComponent GetOnePrior(IComponent component, ComponentDescriptor descriptor)
        {
            if (descriptor == null)
                throw new NullReferenceException("Descriptor is not set");

            var index = _components.IndexOf(component);
            if (index < 0)
            {
                throw new UnknownError(
                    "ComponentNotFound",
                    "Current component  " + component + " was not found in the component list"
                    );
            }

            // Search down from the current component
            for (var i = index - 1; i >= 0; i--)
            {
                var thisComponent = _components[i];
                if (thisComponent.Descriptor.Match(descriptor))
                    return thisComponent;
            }

            // Throw exception if nothing was found
            throw new ConfigError(
                "NoDependency",
                "Compoment " + descriptor + " must be present to satisfy dependencies"
                ).WithDetails(descriptor.ToString());
        }

        /// <summary>
        ///     Creates a component list from components passed as params
        /// </summary>
        /// <param name="components">a list of components</param>
        /// <returns>created component list</returns>
        public static ComponentSet FromComponents(params IComponent[] components)
        {
            var result = new ComponentSet();
            foreach (var component in components)
            {
                result.Add(component);
            }
            return result;
        }
    }
}