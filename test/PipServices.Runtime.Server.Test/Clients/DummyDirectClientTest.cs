using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PipServices.Dummy.Clients;
using PipServices.Dummy.Logic;
using PipServices.Dummy.Persistence;
using PipServices.Runtime.Config;
using PipServices.Runtime.Run;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Clients
{
    [TestClass]
    public class DummyDirectClientTest
    {
        private readonly DummyDirectClient Client;
        private readonly ComponentSet Components;
        private readonly DummyController Ctrl;
        private readonly DummyMemoryPersistence Db;
        private readonly DummyClientFixture Fixture;

        public DummyDirectClientTest()
        {
            Db = new DummyMemoryPersistence();
            Db.Configure(new ComponentConfig());

            Ctrl = new DummyController();
            Ctrl.Configure(new ComponentConfig());

            Client = new DummyDirectClient();
            Client.Configure(new ComponentConfig());

            Components = ComponentSet.FromComponents(Db, Ctrl, Client);

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
    }
}