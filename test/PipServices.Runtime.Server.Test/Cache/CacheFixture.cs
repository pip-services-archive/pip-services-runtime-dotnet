using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PipServices.Runtime.Cache
{
    public class CacheFixture
    {
        private readonly ICache _cache;

        public CacheFixture(ICache cache)
        {
            _cache = cache;
        }

        public void TestBasicOperations()
        {
            // Set the first value
            var value = _cache.Store("test", 123);
            Assert.AreEqual(123, value);

            value = _cache.Retrieve("test");
            Assert.AreEqual(123, value);

            // Set null value
            value = _cache.Store("test", null);
            Assert.IsNull(value);

            value = _cache.Retrieve("test");
            Assert.IsNull(value);

            // Set the second value
            value = _cache.Store("test", "ABC");
            Assert.AreEqual("ABC", value);

            value = _cache.Retrieve("test");
            Assert.AreEqual("ABC", value);

            // Unset value
            _cache.Remove("test");

            value = _cache.Retrieve("test");
            Assert.IsNull(value);
        }

        public void TestReadAfterTimeout(int timeout)
        {
            // Set value
            var value = _cache.Store("test", 123);
            Assert.AreEqual(123, value);

            // Wait
            Thread.Sleep(timeout / 10);

            // Read the value
            value = _cache.Retrieve("test");
            Assert.AreEqual(123, value);

            // Wait
            Thread.Sleep(timeout);

            // Read the value again
            value = _cache.Retrieve("test");
            Assert.IsNull(value);
        }
    }
}