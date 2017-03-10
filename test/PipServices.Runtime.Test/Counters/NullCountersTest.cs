using Microsoft.VisualStudio.TestTools.UnitTesting;
using PipServices.Runtime.Config;
using PipServices.Runtime.Logs;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Counters
{
    [TestClass]
    public class NullCountersTest
    {
        private NullCounters Counters { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            ILogger log = new ConsoleLogger();

            Counters = new NullCounters();
            Counters.Configure(new ComponentConfig());
            Counters.Link(new DynamicMap(), ComponentSet.FromComponents(log));
            Counters.Open();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Counters.Close();
        }

        [TestMethod]
        public void TestSimpleCounters()
        {
            Counters.Last("Test.LastValue", 123);
            Counters.Increment("Test.Increment", 3);
            Counters.Stats("Test.Statistics", 123);
        }

        [TestMethod]
        public void TestMeasureElapsedTime()
        {
            var timer = Counters.BeginTiming("Test.Elapsed");
            timer.EndTiming();
        }
    }
}