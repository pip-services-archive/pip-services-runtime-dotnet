using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PipServices.Dummy.Data;
using PipServices.Dummy.Persistence;
using PipServices.Runtime.Config;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Persistence
{
    [TestClass]
    public class DummyFilePersistenceTest
    {
        private static readonly ComponentConfig Config = ComponentConfig.FromTuples(
            "descriptor.type", "file",
            "options.path", "dummies.json",
            "options.data", new List<DummyObject>()
            );

        private static DummyFilePersistence Db { get; set; }
        private static DummyPersistenceFixture Fixture { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Db = new DummyFilePersistence();
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

        [TestMethod]
        public void TestMultithreading()
        {
            var task = Fixture.TestMultithreading(CancellationToken.None);
            task.Wait();
        }
    }
}