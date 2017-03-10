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
    public class DummyWcfTcpClientTest
    {
        private static readonly ComponentConfig WcfOptions = ComponentConfig.FromTuples(
            "descriptor.type", "wcf",
            "endpoint.protocol", "tcp",
            "endpoint.host", "localhost",
            "endpoint.port", 3001
            );

        private readonly DummyWcfService Api;
        private readonly DummyWcfClient Client;
        private readonly ComponentSet Components;
        private readonly DummyController Ctrl;

        private readonly DummyMemoryPersistence Db;
        private readonly DummyClientFixture Fixture;

        public DummyWcfTcpClientTest()
        {
            Db = new DummyMemoryPersistence();
            Db.Configure(new ComponentConfig());

            Ctrl = new DummyController();
            Ctrl.Configure(new ComponentConfig());

            Api = new DummyWcfService();
            Api.Configure(WcfOptions);

            Client = new DummyWcfClient();
            Client.Configure(WcfOptions);

            Components = ComponentSet.FromComponents(Db, Ctrl, Api, Client);

            Fixture = new DummyClientFixture(Client);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            LifeCycleManager.LinkAndOpen(new DynamicMap(), Components);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            LifeCycleManager.Close(Components);
        }

        [TestMethod]
        public void TestCrudOperations()
        {
            var task = Fixture.TestCrudOperations(CancellationToken.None);
            task.Wait();
        }

        [TestMethod]
        public void TestMultithreading()
        {
            var task = Fixture.TestMultithreading(CancellationToken.None);
            task.Wait();
        }
    }
}