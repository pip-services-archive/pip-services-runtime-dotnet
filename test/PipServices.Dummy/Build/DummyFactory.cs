using PipServices.Dummy.Clients;
using PipServices.Dummy.Logic;
using PipServices.Dummy.Persistence;
using PipServices.Dummy.Services;
using PipServices.Runtime.Build;
using PipServices.Runtime.Logs;

namespace PipServices.Dummy.Build
{
    public class DummyFactory : ComponentFactory
    {
        public static readonly DummyFactory Instance = new DummyFactory();

        public DummyFactory()
            : base(DefaultFactory.Instance)
        {
            Register(DummyMongoDbPersistance.ClassDescriptor, typeof(DummyMongoDbPersistance));
            Register(DummyFilePersistence.ClassDescriptor, typeof(DummyFilePersistence));
            Register(DummyMemoryPersistence.ClassDescriptor, typeof(DummyMemoryPersistence));
            Register(DummyController.ClassDescriptor, typeof(DummyController));
            Register(DummyWcfService.ClassDescriptor, typeof(DummyWcfService));
            Register(DummyRestService.ClassDescriptor, typeof(DummyRestService));
            Register(DummyWcfClient.ClassDescriptor, typeof(DummyWcfClient));
            Register(DummyRestClient.ClassDescriptor, typeof(DummyRestClient));
            Register(DummyDirectClient.ClassDescriptor, typeof(DummyDirectClient));
        }
    }
}