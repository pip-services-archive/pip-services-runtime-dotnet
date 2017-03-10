using PipServices.Runtime.Config;

namespace PipServices.Runtime.Cache
{
    public class NullCache : AbstractCache
    {
        /// <summary>
        ///     Unique descriptor for the Memory Cache component
        /// </summary>
        public static readonly ComponentDescriptor ClassDescriptor = new ComponentDescriptor(
            Category.Cache, "pip-services-runtime-cache", "null", "*"
            );

        public NullCache()
            : base(ClassDescriptor)
        {
        }

        public override object Retrieve(string key)
        {
            return null;
        }

        public override object Store(string key, object value)
        {
            return value;
        }

        public override void Remove(string key)
        {
        }
    }
}