using System;
using System.Runtime.Caching;
using PipServices.Runtime.Config;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Cache
{
    public class MemoryCache : AbstractCache
    {
        /// <summary>
        ///     Unique descriptor for the Memory Cache component
        /// </summary>
        public static readonly ComponentDescriptor ClassDescriptor = new ComponentDescriptor(
            Category.Cache, "pip-services-runtime-cache", "memory", "*"
            );

        private static readonly DynamicMap DefaultConfig = DynamicMap.FromTuples(
            "options.timeout", 60000, // timeout in milliseconds
            "options.max_size", 1000 // muximum number of elements in cache
            );

        private readonly System.Runtime.Caching.MemoryCache _standardCache = System.Runtime.Caching.MemoryCache.Default;
        private CacheItemPolicy _policy;

        //private readonly Dictionary<string, CacheEntry> _cache = new Dictionary<string, CacheEntry>();
        //private int _count;
        private int _maxSize;
        private long _timeout;

        public MemoryCache()
            : base(ClassDescriptor)
        {
        }

        public override void Configure(ComponentConfig config)
        {
            CheckNewStateAllowed(State.Configured);

            base.Configure(config.WithDefaults(DefaultConfig));

            _timeout = _config.Options.GetLong("timeout");
            _maxSize = _config.Options.GetInteger("max_size");

            _policy = new CacheItemPolicy()
            {
                SlidingExpiration = TimeSpan.FromMilliseconds(_timeout)
            };
        }

        public override object Retrieve(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return _standardCache.Get(key);
        }

        public override object Store(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            // Shortcut to remove entry from the cache
            if (value == null)
            {
                if (_standardCache.Contains(key))
                    _standardCache.Remove(key);

                return null;
            }

            if (_maxSize <= _standardCache.GetCount())
                lock (_standardCache)
                {
                    if (_maxSize <= _standardCache.GetCount())
                        _standardCache.Trim(5);
                }

            _standardCache.Set(key, value, _policy);

            return value;
        }

        public override void Remove(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            _standardCache.Remove(key);
        }
    }
}