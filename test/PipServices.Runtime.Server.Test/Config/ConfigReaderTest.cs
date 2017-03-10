using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PipServices.Runtime.Config
{
    [TestClass]
    public class ConfigReaderTest
    {
        [TestMethod]
        public void TestJson()
        {
            var config = ConfigReader.Read("Config\\options.json");
            var content = config.RawContent;

            Assert.IsNotNull(config);
            Assert.AreEqual(123, content.GetInteger("test"));
        }

        [TestMethod]
        public void TestYaml()
        {
            var config = ConfigReader.Read("Config\\options.yaml");
            var content = config.RawContent;

            Assert.IsNotNull(config);
            Assert.AreEqual(123, content.GetInteger("test"));
        }
    }
}