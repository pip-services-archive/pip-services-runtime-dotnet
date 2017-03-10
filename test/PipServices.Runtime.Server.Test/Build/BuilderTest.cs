using Microsoft.VisualStudio.TestTools.UnitTesting;
using PipServices.Dummy.Build;
using PipServices.Runtime.Config;

namespace PipServices.Runtime.Build
{
    [TestClass]
    public class BuilderTest
    {
        [TestMethod]
        public void TestDynamicComponents()
        {
            var config = MicroserviceConfig.FromTuples(
                "logs.descriptor.type", "console",
                "logs.constructor.assembly", "PipServices.Runtime.dll",
                "logs.constructor.class", "PipServices.Runtime.Logs.ConsoleLogger",
                "counters.constructor.class", "PipServices.Runtime.Counters.LogCounters, PipServices.Runtime"
                );

            var components = Builder.Build(DummyFactory.Instance, config);
            Assert.AreEqual(3, components.GetAllOrdered().Count);
        }

        [TestMethod]
        public void TestDynamicFactories()
        {
            var config = MicroserviceConfig.FromTuples(
                "factories.constructor.assembly", "PipServices.Dummy.dll",
                "factories.constructor.class", "PipServices.Dummy.Build.DummyFactory",
                "persistence.descriptor.group", "pip-services-dummies",
                "persistence.descriptor.type", "memory"
                );

            var components = Builder.Build(DefaultFactory.Instance, config);
            Assert.AreEqual(4, components.GetAllOrdered().Count);
        }
    }
}