namespace PipServices.Runtime
{
    /// <summary>
    ///     Transient cache which is used to bypass persistence
    ///     to increase overall system performance.
    /// </summary>
    public interface ICache : IComponent
    {
        /// <summary>
        ///     Retrieves a value from the cache by unique key.
        ///     It is recommended to use either string GUIDs like '123456789abc'
        ///     or unique natural keys prefixed with the functional group
        ///     like 'pip-services-storage:block-123'.
        /// </summary>
        /// <param name="key">a unique key to locate value in the cache</param>
        /// <returns>a cached value or <b>null</b> if value wasn't found or timeout expired.</returns>
        object Retrieve(string key);

        /// <summary>
        ///     Stores value identified by unique key in the cache.
        ///     Stale timeout is configured in the component options.
        /// </summary>
        /// <param name="key">a unique key to locate value in the cache.</param>
        /// <param name="value">a value to store.</param>
        /// <returns>a cached value stored in the cache.</returns>
        object Store(string key, object value);

        /// <summary>
        ///     Removes value stored in the cache.
        /// </summary>
        /// <param name="key">a unique key to locate value in the cache.</param>
        void Remove(string key);
    }
}