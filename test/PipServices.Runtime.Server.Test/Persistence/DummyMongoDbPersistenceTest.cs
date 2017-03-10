using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PipServices.Dummy.Persistence;
using PipServices.Runtime.Config;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Persistence
{
    [TestClass]
    public class DummyMongoDbPersistenceTest
    {
        private static DummyMongoDbPersistance Db { get; set; }
        private static DummyPersistenceFixture Fixture { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var config = ConfigReader.Read("..\\..\\..\\..\\config\\config.yaml");
            var dbConfigs = config.GetSection(Category.Persistence);
            var dbConfig = dbConfigs.SingleOrDefault(c => c.Descriptor.Type == "mongodb");

            if (dbConfig != null)
            {
                Db = new DummyMongoDbPersistance();
                Fixture = new DummyPersistenceFixture(Db);

                Db.Configure(dbConfig);
                Db.Link(new DynamicMap(), new ComponentSet());
                Db.Open();
            }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            if (Db != null)
                Db.Close();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            if (Db != null)
                Db.Clear();
        }

        [TestMethod]
        public void TestCrudOperations()
        {
            if (Fixture == null) return;

            var task = Fixture.TestCrudOperations(CancellationToken.None);
            task.Wait();
        }
    }
}