using System;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Config
{
    /// <summary>
    ///     Dynamic constructor configuration parameters: Assembly and Class names
    /// </summary>
    public class ComponentConstructor
    {
        /// <summary>
        ///     Create an empty instance of a constructor
        /// </summary>
        public ComponentConstructor()
        {
            RawContent = new DynamicMap();
        }

        /// <summary>
        ///     Create an instance of constructor with free-form configuration map.
        /// </summary>
        /// <param name="content">a map with the constructor configuration parameters. </param>
        public ComponentConstructor(DynamicMap content)
        {
            if (content == null)
                throw new NullReferenceException("Content is not set");

            RawContent = content;
        }

        /// <summary>
        ///     Endpoint as free-form configuration set.
        /// </summary>
        public DynamicMap RawContent { get; }

        /// <summary>
        ///     Checks is constructor is defined
        /// </summary>
        public bool IsDefined
        {
            get { return Assembly != null || Class != null; }
        }

        /// <summary>
        ///     Assembly that contains the component
        /// </summary>
        public string Assembly
        {
            get
            {
                var assembly = RawContent.GetNullableString("assembly");
                assembly = assembly != null ? assembly : RawContent.GetNullableString("module");
                assembly = assembly != null ? assembly : RawContent.GetNullableString("jar");
                return assembly;
            }
        }

        /// <summary>
        ///     Class of the component
        /// </summary>
        public string Class
        {
            get
            {
                var clazz = RawContent.GetNullableString("class");
                clazz = clazz != null ? clazz : RawContent.GetNullableString("entry");
                return clazz;
            }
        }

        public override string ToString()
        {
            var result = "" + Class;
            var assembly = Assembly;
            if (assembly != null)
                result += ", " + assembly;
            return result;
        }
    }
}