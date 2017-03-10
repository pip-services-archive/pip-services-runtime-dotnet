using Microsoft.VisualStudio.TestTools.UnitTesting;
using PipServices.Runtime.Config;
using PipServices.Runtime.Logs;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Counters
{
    [TestClass]
    public class LogCountersTest
    {
        private LogCounters Counters { get; set; }
        private CountersFixture Fixture { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            ILogger log = new ConsoleLogger();

            Counters = new LogCounters();
            Counters.Configure(new ComponentConfig());
            Counters.Link(new DynamicMap(), ComponentSet.FromComponents(log));
            Counters.Open();

            Fixture = new CountersFixture(Counters);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Counters.Close();
        }

        [TestMethod]
        public void TestSimpleCounters()
        {
            Fixture.TestSimpleCounters();
        }

        [TestMethod]
        public void TestMeasureElapsedTime()
        {
            Fixture.TestMeasureElapsedTime();
        }
    }
}