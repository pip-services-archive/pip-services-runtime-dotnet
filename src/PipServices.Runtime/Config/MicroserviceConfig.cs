using System;
using System.Collections.Generic;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Config
{
    /// <summary>
    ///     Configuration for the entire microservice.
    ///     It can be either stored in JSON file on disk,
    ///     kept in remote configuration registry or hardcoded within test.
    /// </summary>
    public class MicroserviceConfig
    {
        /// <summary>
        ///     Creates an empty instance of the microservice configuration.
        ///     It can be filled with data manually or loaded from the file.
        /// </summary>
        public MicroserviceConfig()
        {
            RawContent = new DynamicMap();
        }

        /// <summary>
        ///     Creates instance of the microservice configuration and
        ///     initializes it with data from dynamic map.
        /// </summary>
        /// <param name="content"></param>
        public MicroserviceConfig(DynamicMap content)
        {
            if (content == null)
                throw new NullReferenceException("Content is not empty");
            RawContent = content;
        }

        /// <summary>
        ///     Creates instance of the microservice configuration and
        ///     initializes it with hardcoded parameters.
        ///     The parameters shall be specified with key/value tuples as:
        ///     'key1', value1, 'key2', value2, ...
        /// </summary>
        /// <param name="values">list of parameters with key-value tuples </param>
        public MicroserviceConfig(params object[] values)
        {
            RawContent = new DynamicMap();
            RawContent.SetTuplesArray(values);
        }

        /// <summary>
        ///     Gets the raw content of the configuration as dynamic map
        /// </summary>
        public DynamicMap RawContent { get; }

        /// <summary>
        ///     Gets configurations of components for specific section.
        /// </summary>
        /// <param name="category">a category that defines a section within microservice configuration</param>
        /// <returns>an array with components configurations</returns>
        public List<ComponentConfig> GetSection(string category)
        {
            var configs = new List<ComponentConfig>();

            var values = RawContent.GetArray(category);
            foreach (var value in values)
            {
                var config = new ComponentConfig(category, DynamicMap.FromValue(value));
                configs.Add(config);
            }
            return configs;
        }

        /// <summary>
        ///     Removes specified sections from the configuration.
        ///     This method can be used to suppress certain functionality in the microservice
        ///     like api services when microservice runs inside Lambda function.
        /// </summary>
        /// <param name="categories">a list of categories / section names to be removed.</param>
        public void RemoveSections(params string[] categories)
        {
            foreach (var category in categories)
            {
                RawContent.Remove(category);
            }
        }

        /// <summary>
        ///     Creates microservice configuration using free-form objects.
        /// </summary>
        /// <param name="value">a free-form object</param>
        /// <returns>constructed microservice configuration</returns>
        public static MicroserviceConfig FromValue(object value)
        {
            var content = DynamicMap.FromValue(value);
            return new MicroserviceConfig(content);
        }

        /// <summary>
        ///     Creates component configuration using hardcoded parameters.
        ///     This method of configuration is usually used during testing.
        ///     The configuration is created with 'Undefined' category
        ///     since it's not used to create a component.
        /// </summary>
        /// <param name="tuples">
        ///     configuration parameters as
        ///     <key> +
        ///         <value> tuples
        /// </param>
        /// <returns>constructed microservice configuration</returns>
        public static MicroserviceConfig FromTuples(params object[] tuples)
        {
            var content = DynamicMap.FromTuples(tuples);
            return new MicroserviceConfig(content);
        }
    }
}