using PipServices.Runtime.Config;

namespace PipServices.Dummy.Persistence
{
    public class DummyMemoryPersistence : DummyFilePersistence
    {
        /// <summary>
        ///     Unique descriptor for the DummyMemoryPersistence component
        /// </summary>
        public new static readonly ComponentDescriptor ClassDescriptor = new ComponentDescriptor(
            Category.Persistence, "pip-services-dummies", "memory", "*"
            );

        public DummyMemoryPersistence()
            : base(ClassDescriptor)
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