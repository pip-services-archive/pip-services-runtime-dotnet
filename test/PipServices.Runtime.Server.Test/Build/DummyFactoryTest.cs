using Microsoft.VisualStudio.TestTools.UnitTesting;
using PipServices.Dummy.Build;
using PipServices.Dummy.Data;
using PipServices.Runtime.Config;

namespace PipServices.Runtime.Build
{
    [TestClass]
    public class DummyFactoryTest
    {
        private readonly MicroserviceConfig BuildConfig = MicroserviceConfig.FromTuples(
            "logs.descriptor.type", "console",
            "counters.descriptor.type", "log",
            "cache.descriptor.type", "memory",
            "persistence.descriptor.type", "file",
            "persistence.options.path", "..\\..\\..\\..\\data\\dummies.json",
            "persistence.options.data", new DummyObject[0],
            "controllers.descriptor.type", "*",
            "services.descriptor.type", "rest",
            "services.descriptor.version", "1.0"
            );

        [TestMethod]
        public void TestBuildDefaults()
        {
            var config = new MicroserviceConfig();
            var components = Builder.Build(DummyFactory.Instance, config);
            Assert.AreEqual(3, components.GetAllOrdered().Count);
        }

        [TestMethod]
        public void TestStartWithConfig()
        {
            var components = Builder.Build(DummyFactory.Instance, BuildConfig);
            Assert.AreEqual(6, components.GetAllOrdered().Count);
        }

        [TestMethod]
        public void TestStartWithFile()
        {
            var config = ConfigReader.Read("Build\\config.json");
            var components = Builder.Build(DummyFactory.Instance, config);
            Assert.AreEqual(9, components.GetAllOrdered().Count);
        }
    }
}