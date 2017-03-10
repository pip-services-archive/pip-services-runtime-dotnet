namespace PipServices.Runtime
{
    /// <summary>
    ///     State in lifecycle of components or the entire microservice
    /// </summary>
    public enum State
    {
        /// <summary>
        ///     Undefined state
        /// </summary>
        Undefined = -1,

        /// <summary>
        ///     Initial state right after creation
        /// </summary>
        Initial = 0,

        /// <summary>
        ///     Configuration was successfully completed
        /// </summary>
        Configured = 1,

        /// <summary>
        ///     Links between components were successfully set
        /// </summary>
        Linked = 2,

        /// <summary>
        ///     Opening
        /// </summary>
        Opening = 3,

        /// <summary>
        ///     Ready to perform operations
        /// </summary>
        Opened = 4,

        /// <summary>
        ///     Ready to perform operations. This is a duplicate for Opened.
        /// </summary>
        Ready = 4,

        /// <summary>
        ///     Closed but can be reopened
        /// </summary>
        Closed = 5
    }
}