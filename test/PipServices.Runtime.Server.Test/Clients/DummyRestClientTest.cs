using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PipServices.Dummy.Clients;
using PipServices.Dummy.Logic;
using PipServices.Dummy.Persistence;
using PipServices.Dummy.Services;
using PipServices.Runtime.Config;
using PipServices.Runtime.Run;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Clients
{
    [TestClass]
    public class DummyRestClientTest
    {
        private static readonly ComponentConfig WcfOptions = ComponentConfig.FromTuples(
            "descriptor.type", "rest",
            "endpoint.protocol", "http",
            "endpoint.host", "localhost",
            "endpoint.port", 3000
            );

        private readonly DummyRestService _api;
        private readonly DummyRestClient _client;
        private readonly ComponentSet _components;
        private readonly DummyController _ctrl;

        private readonly DummyMemoryPersistence _db;
        private readonly DummyClientFixture _fixture;

        public DummyRestClientTest()
        {
            _db = new DummyMemoryPersistence();
            _db.Configure(new ComponentConfig());

            _ctrl = new DummyController();
            _ctrl.Configure(new ComponentConfig());

            _api = new DummyRestService();
            _api.Configure(WcfOptions);

            _client = new DummyRestClient();
            _client.Configure(WcfOptions);

            _components = ComponentSet.FromComponents(_db, _ctrl, _client, _api);

            _fixture = new DummyClientFixture(_client);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            LifeCycleManager.LinkAndOpen(new DynamicMap(), _components);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            LifeCycleManager.Close(_components);
        }

        [TestMethod]
        public void TestCrudOperations()
        {
            var task = _fixture.TestCrudOperations(CancellationToken.None);
            task.Wait();
        }
    }
}