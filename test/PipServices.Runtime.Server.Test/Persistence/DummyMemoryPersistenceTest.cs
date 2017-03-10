using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PipServices.Dummy.Persistence;
using PipServices.Runtime.Config;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Persistence
{
    [TestClass]
    public class DummyMemoryPersistenceTest
    {
        private static readonly ComponentConfig Config = ComponentConfig.FromTuples(
            "descriptor.type", "memory"
            );

        private static DummyMemoryPersistence Db { get; set; }
        private static DummyPersistenceFixture Fixture { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Db = new DummyMemoryPersistence();
            Fixture = new DummyPersistenceFixture(Db);

            Db.Configure(Config);
            Db.Link(new DynamicMap(), new ComponentSet());
            Db.Open();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Db.Close();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Db.Clear();
        }

        [TestMethod]
        public void TestCrudOperations()
        {
            var task = Fixture.TestCrudOperations(CancellationToken.None);
            task.Wait();
        }
    }
}