using System;
using System.Collections.Generic;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Config
{
    /// <summary>
    ///     Stores configuration for microservice component
    /// </summary>
    public class ComponentConfig
    {
        private readonly string _category;

        /// <summary>
        ///     Creates an empty component configuration
        ///     This constructor is used in testing
        /// </summary>
        public ComponentConfig()
        {
            _category = Category.Undefined;
            RawContent = new DynamicMap();
        }

        /// <summary>
        ///     Creates instance of component configuration with values
        ///     retrieved from microservice configuration section.
        /// </summary>
        /// <param name="category">a component category</param>
        /// <param name="content">configuration parameters</param>
        public ComponentConfig(string category, DynamicMap content)
        {
            if (category == null)
                throw new NullReferenceException("Category is not set");
            if (content == null)
                throw new NullReferenceException("Content is empty");

            _category = category;
            RawContent = content;
        }

        /// <summary>
        ///     Gets the raw content of the configuration as dynamic map
        /// </summary>
        public DynamicMap RawContent { get; private set; }

        /// <summary>
        ///     Gets component descriptor. It is read from 'descriptor' object if it exists.
        /// </summary>
        public ComponentDescriptor Descriptor
        {
            get
            {
                var values = RawContent.GetMap("descriptor");

                return new ComponentDescriptor(
                    _category,
                    values.GetNullableString("group"),
                    values.GetNullableString("type"),
                    values.GetNullableString("version")
                    );
            }
        }

        /// <summary>
        ///     Gets component dynamic constructor. It is read from 'constructor' object is it exists.
        ///     If the 'constructor' is absent in configuration the property returns <code>null</code>.
        /// </summary>
        public ComponentConstructor Constructor
        {
            get
            {
                var values = RawContent.GetNullableMap("constructor");
                return values != null ? new ComponentConstructor(values) : null;
            }
        }

        /// <summary>
        ///     Gets connection parameters from 'connection' object
        ///     or with parameters from the root object.
        ///     This method is usually used by persistence components
        ///     to get connections to databases.
        /// </summary>
        public Connection Connection
        {
            get
            {
                var values = RawContent.GetNullableMap("connection");
                return values != null ? new Connection(values) : null;
            }
        }

        /// <summary>
        ///     Gets a list of database connections from 'connections' or 'connection' objects
        ///     This method is usually used by clients that may connect to one of few services.
        /// </summary>
        public IList<Connection> Connections
        {
            get
            {
                // Get configuration parameters for connections
                var values = RawContent.GetNullableArray("connections");
                values = values != null ? values : RawContent.GetNullableArray("connection");

                // Convert configuration parameters to connections
                IList<Connection> connections = new List<Connection>();

                // Convert list of values
                if (values != null)
                {
                    foreach (var value in values)
                    {
                        connections.Add(new Connection(DynamicMap.FromValue(value)));
                    }
                }

                // Return the result
                return connections;
            }
        }

        /// <summary>
        ///     Gets a service endpoint from 'endpoint' object
        ///     This method is usually used by services that need to bind to a single endpoint.
        /// </summary>
        public Endpoint Endpoint
        {
            get
            {
                var values = RawContent.GetNullableMap("endpoint");
                return values != null ? new Endpoint(values) : null;
            }
        }

        /// <summary>
        ///     Gets a list of service endpoint from 'endpoints' or 'endpoint' objects
        ///     This method is usually used by clients that may connect to one of few services.
        /// </summary>
        public IList<Endpoint> Endpoints
        {
            get
            {
                // Get configuration parameters for endpoints
                var values = RawContent.GetNullableArray("endpoints");
                values = values != null ? values : RawContent.GetNullableArray("endpoint");

                // Convert configuration parameters to endpoints
                IList<Endpoint> endpoints = new List<Endpoint>();

                // Convert list of values
                if (values != null)
                {
                    foreach (var value in values)
                    {
                        endpoints.Add(new Endpoint(DynamicMap.FromValue(value)));
                    }
                }

                // Return the result
                return endpoints;
            }
        }

        /// <summary>
        ///     Gets component free-form configuration options.
        ///     The options are read from 'options', 'settings' or 'params' objects.
        /// </summary>
        public DynamicMap Options
        {
            get { return RawContent.GetNullableMap("options"); }
        }

        /// <summary>
        ///     Sets default values to the configuration
        /// </summary>
        /// <param name="defaultContent">default configuration</param>
        /// <returns>a reference to this configuration for chaining or passing through.</returns>
        public ComponentConfig WithDefaults(DynamicMap defaultContent)
        {
            RawContent = RawContent.Merge(defaultContent, true);
            return this;
        }

        /// <summary>
        ///     Sets default values to the configuration
        /// </summary>
        /// <param name="defaultValues">default configuration</param>
        /// <returns>a reference to this configuration for chaining or passing through.</returns>
        public ComponentConfig WithDefaultValues(params object[] defaultValues)
        {
            var defaultContent = new DynamicMap();
            defaultContent.SetTuplesArray(defaultValues);
            return WithDefaults(defaultContent);
        }

        /// <summary>
        ///     Creates component configuration using free-form objects.
        ///     This method of configuration is usually used during testing.
        ///     The configuration is created with 'Undefined' category
        ///     since it's not used to create a component.
        /// </summary>
        /// <param name="value">a free-form object</param>
        /// <returns>a constructed component configuration</returns>
        public static ComponentConfig FromValue(object value)
        {
            var content = DynamicMap.FromValue(value);
            return new ComponentConfig(Category.Undefined, content);
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
        /// <returns>constructed component configuration</returns>
        public static ComponentConfig FromTuples(params object[] tuples)
        {
            var content = DynamicMap.FromTuples(tuples);
            return new ComponentConfig(Category.Undefined, content);
        }
    }
}