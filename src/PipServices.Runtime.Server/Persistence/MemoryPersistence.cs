using PipServices.Runtime.Config;
using PipServices.Runtime.Data;

namespace PipServices.Runtime.Persistence
{
    public abstract class MemoryPersistence<T> : FilePersistence<T> where T : IIdentifiable
    {
        protected MemoryPersistence(ComponentDescriptor descriptor)
            : base(descriptor)
        {
        }

        public override void Configure(ComponentConfig config)
        {
            base.Configure(config.WithDefaultValues("options.path", ""));
        }

        public override void Save()
        {
            // SKip saving data to disk
        }
    }
}