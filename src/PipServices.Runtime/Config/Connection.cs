using System;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Config
{
    /// <summary>
    ///     Database connection configuration as set in the component config.
    ///     It usually contains a complete uri or separate host, port, user, password, etc.
    /// </summary>
    public class Connection
    {
        /// <summary>
        ///     Creates an empty instance of connection.
        /// </summary>
        public Connection()
        {
            RawContent = new DynamicMap();
        }

        /// <summary>
        ///     Create an instance of database connection with free-form configuration map.
        /// </summary>
        /// <param name="content">a map with the connection configuration parameters.</param>
        public Connection(DynamicMap content)
        {
            if (content == null)
                throw new NullReferenceException("Content is not set");

            RawContent = content;
        }

        /// <summary>
        ///     Connection as free-form configuration set.
        /// </summary>
        public DynamicMap RawContent { get; }

        /// <summary>
        ///     The connection type
        /// </summary>
        public string Type
        {
            get { return RawContent.GetNullableString("type"); }
        }

        /// <summary>
        ///     Gets the connection host name or ip address.
        /// </summary>
        public string Host
        {
            get { return RawContent.GetNullableString("host"); }
        }

        /// <summary>
        ///     Gets the connection port number
        /// </summary>
        public int Port
        {
            get { return RawContent.GetInteger("port"); }
        }

        /// <summary>
        ///     Gets the database name
        /// </summary>
        public string Database
        {
            get { return RawContent.GetNullableString("database"); }
        }

        /// <summary>
        ///     Gets the connection user name
        /// </summary>
        public string Username
        {
            get { return RawContent.GetNullableString("username"); }
        }

        /// <summary>
        ///     Gets the connection user password
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
            get { return Type + "://" + Host + ":" + Port; }
        }
    }
}