using System;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Config
{
    /// <summary>
    ///     Service address as set in component configuration or
    ///     retrieved by discovery service.It contains service protocol,
    ///     host, port number, timeouts and additional configuration parameters.
    /// </summary>
    public class Endpoint
    {
        /// <summary>
        ///     Create an empty instance of a service endpoint
        /// </summary>
        public Endpoint()
        {
            RawContent = new DynamicMap();
        }

        /// <summary>
        ///     Create an instance of service endpoint with free-form configuration map.
        /// </summary>
        /// <param name="content">a map with the address configuration parameters. </param>
        public Endpoint(DynamicMap content)
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
        ///     Checks if discovery registration or resolution shall be performed.
        ///     The discovery is requested when 'discover' parameter contains
        ///     a non-empty string that represents the discovery name.
        /// </summary>
        public bool UseDiscovery
        {
            get { return RawContent.Has("discover") || RawContent.Has("discovery"); }
        }

        /// <summary>
        ///     Gets a name under which the address shall be registered or resolved by discovery service.
        /// </summary>
        public string DiscoveryName
        {
            get
            {
                var discover = RawContent.GetNullableString("discover");
                discover = discover != null ? discover : RawContent.GetNullableString("discovery");
                return discover;
            }
        }

        /// <summary>
        ///     The endpoint protocol
        /// </summary>
        public string Protocol
        {
            get { return RawContent.GetNullableString("protocol"); }
        }

        /// <summary>
        ///     Gets the service host name or ip address.
        /// </summary>
        public string Host
        {
            get
            {
                var host = RawContent.GetNullableString("host");
                host = host != null ? host : RawContent.GetNullableString("ip");
                return host;
            }
        }

        /// <summary>
        ///     Gets the service port number
        /// </summary>
        public int Port
        {
            get { return RawContent.GetInteger("port"); }
        }

        /// <summary>
        ///     Gets the service user name
        /// </summary>
        public string Username
        {
            get { return RawContent.GetNullableString("username"); }
        }

        /// <summary>
        ///     Gets the service user password
        /// </summary>
        public string Password
        {
            get { return RawContent.GetNullableString("password"); }
        }

        /// <summary>
        ///     Gets the endpoint uri constructed from protocol, host and port
        /// </summary>
        public string Uri
        {
            get { return Protocol + "://" + Host + ":" + Port; }
        }
    }
}