using Microsoft.VisualStudio.TestTools.UnitTesting;
using PipServices.Runtime.Config;

namespace PipServices.Runtime.Cache
{
    [TestClass]
    public class MemoryCacheTest
    {
        private ICache Cache { get; set; }
        private CacheFixture Fixture { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            var config = ComponentConfig.FromTuples("options.timeout", 500);
            Cache = new MemoryCache();
            Cache.Configure(config);
            Fixture = new CacheFixture(Cache);
        }

        [TestMethod]
        public void TestBasicOperations()
        {
            Fixture.TestBasicOperations();
        }

        [TestMethod]
        public void TestReadAfterTimeout()
        {
            Fixture.TestReadAfterTimeout(1000);
        }
    }
}