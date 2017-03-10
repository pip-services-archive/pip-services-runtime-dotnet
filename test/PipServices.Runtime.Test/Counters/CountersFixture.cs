using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PipServices.Runtime.Counters
{
    public class CountersFixture
    {
        private readonly AbstractCounters _counters;

        public CountersFixture(AbstractCounters counters)
        {
            _counters = counters;
        }

        public void TestSimpleCounters()
        {
            _counters.Last("Test.LastValue", 123);
            _counters.Last("Test.LastValue", 123456);

            var counter = _counters.Get("Test.LastValue", CounterType.LastValue);
            Assert.IsNotNull(counter);
            Assert.AreEqual(counter.Last.Value, 123456, 0.001);

            _counters.IncrementOne("Test.Increment");
            _counters.Increment("Test.Increment", 3);

            counter = _counters.Get("Test.Increment", CounterType.Increment);
            Assert.IsNotNull(counter);
            Assert.AreEqual(counter.Count, 4);

            _counters.TimestampNow("Test.Timestamp");
            _counters.TimestampNow("Test.Timestamp");

            counter = _counters.Get("Test.Timestamp", CounterType.Timestamp);
            Assert.IsNotNull(counter);
            Assert.IsTrue(counter.Time.HasValue);

            _counters.Stats("Test.Statistics", 1);
            _counters.Stats("Test.Statistics", 2);
            _counters.Stats("Test.Statistics", 3);

            counter = _counters.Get("Test.Statistics", CounterType.Statistics);
            Assert.IsNotNull(counter);
            Assert.AreEqual(counter.Avg.Value, 2, 0.001);

            _counters.Dump();
        }

        public void TestMeasureElapsedTime()
        {
            using (var timing = _counters.BeginTiming("Test.Elapsed"))
            {
                Thread.Sleep(100);

                timing.EndTiming();

                var counter = _counters.Get("Test.Elapsed", CounterType.Interval);
                Assert.IsNotNull(counter);
                Assert.IsTrue(counter.Last > 50);
                Assert.IsTrue(counter.Last < 5000);

                _counters.Dump();
            }
        }
    }
}