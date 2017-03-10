using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PipServices.Runtime.Config;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Boot
{
    [TestClass]
    public class FileBootConfigTest
    {
        private IBootConfig Config { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            var config = ComponentConfig.FromTuples(
                "options.path", "Boot\\options.json"
                );
            Config = new FileBootConfig();
            Config.Configure(config);
            Config.Link(new DynamicMap(), new ComponentSet());
            Config.Open();
        }

        [TestMethod]
        public void TestRead()
        {
            var config = Config.ReadConfig();
            var options = config.RawContent;

            Assert.IsNotNull(options);
            Assert.AreEqual(123, Converter.ToInteger(options["test"]));

            var array = (options["array"] as IEnumerable<object>).ToArray();
            Assert.IsNotNull(array);
            Assert.AreEqual(111, Converter.ToInteger(array[0]));
            Assert.AreEqual(222, Converter.ToInteger(array[1]));
        }
    }
}