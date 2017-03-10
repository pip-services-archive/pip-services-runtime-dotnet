using System.Text;

namespace PipServices.Runtime.Config
{
    /// <summary>
    ///     Component descriptor used to identify the component by descriptive elements:
    ///     <ul>
    ///         <li>
    ///             logical group: typically microservice with or without transaction subgroup 'pip-services-storage:blocks'
    ///             <li>
    ///                 component category: 'controller', 'services' or 'cache'
    ///                 <li>
    ///                     functional type: 'memory', 'file' or 'mongodb', ...
    ///                     <li> implementation version: '1.0', '1.5' or '10.4'
    ///     </ul>
    ///     The descriptor also checks matching to another descriptor for component search.
    ///     '*' or null mean that element shall match to any value.
    /// </summary>
    public class ComponentDescriptor
    {
        /// <summary>
        ///     Creates instance of a component descriptor
        /// </summary>
        /// <param name="category">component category: 'cache', 'services' or 'controllers' </param>
        /// <param name="group">logical group: 'pip-services-runtime', 'pip-services-logging'</param>
        /// <param name="type">functional type: 'memory', 'file' or 'memcached' </param>
        /// <param name="version">implementation version: '1.0'. '1.5' or '10.4'</param>
        public ComponentDescriptor(string category, string group, string type, string version)
        {
            if ("*".Equals(category)) category = null;
            if ("*".Equals(group)) group = null;
            if ("*".Equals(type)) type = null;
            if ("*".Equals(version)) version = null;

            Category = category;
            Group = group;
            Type = type;
            Version = version;
        }

        /// <summary>
        ///     The component category
        /// </summary>
        public string Category { get; }

        /// <summary>
        ///     Logical group of the component
        /// </summary>
        public string Group { get; }

        /// <summary>
        ///     Functional type of the component
        /// </summary>
        public string Type { get; }

        /// <summary>
        ///     The component version
        /// </summary>
        public string Version { get; }

        public bool Match(ComponentDescriptor descriptor)
        {
            if (Category != null && descriptor.Category != null)
            {
                // Special processing if this category is business logic
                if (Category.Equals(Config.Category.BusinessLogic))
                {
                    if (!descriptor.Category.Equals(Config.Category.Controllers)
                        && !descriptor.Category.Equals(Config.Category.Decorators)
                        && !descriptor.Category.Equals(Config.Category.BusinessLogic))
                        return false;
                }
                // Special processing is descriptor category is business logic
                else if (descriptor.Category.Equals(Config.Category.BusinessLogic))
                {
                    if (!Category.Equals(Config.Category.Controllers)
                        && !Category.Equals(Config.Category.Decorators)
                        && !Category.Equals(Config.Category.BusinessLogic))
                        return false;
                }
                // Matching categories
                else if (!Category.Equals(descriptor.Category))
                {
                    return false;
                }
            }

            // Matching groups
            if (Group != null && descriptor.Group != null
                && !Group.Equals(descriptor.Group))
            {
                return false;
            }

            // Matching types
            if (Type != null && descriptor.Type != null
                && !Type.Equals(descriptor.Type))
            {
                return false;
            }

            // Matching versions
            if (Version != null && descriptor.Version != null
                && !Version.Equals(descriptor.Version))
            {
                return false;
            }

            // All checks are passed...
            return true;
        }

        /// <summary>
        ///     Compares component descriptor to another object (descriptor)
        /// </summary>
        /// <param name="obj">An object to be compared</param>
        /// <returns><b>true</b> if descriptors match each other</returns>
        public override bool Equals(object obj)
        {
            if (obj is ComponentDescriptor)
                return Match((ComponentDescriptor) obj);
            return false;
        }

        /// <summary>
        ///     Returns string representation for the component descriptor.
        /// </summary>
        /// <returns>A string representation of the descriptor</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(Category)
                .Append("/").Append(Group != null ? Group : "*")
                .Append("/").Append(Type != null ? Type : "*")
                .Append("/").Append(Version != null ? Version : "*");
            return builder.ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}