using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PipServices.Runtime.Data
{
    [TestClass]
    public class IdGeneratorTest
    {
        private void TestIds(Func<string> generator, int minSize)
        {
            var id1 = generator();
            Assert.IsNotNull(id1);
            Assert.IsTrue(id1.Length >= minSize);

            var id2 = generator();
            Assert.IsNotNull(id2);
            Assert.IsTrue(id2.Length >= minSize);
            Assert.AreNotEqual(id1, id2);
        }

        [TestMethod]
        public void TestShortId()
        {
            TestIds(IdGenerator.Short, 9);
        }

        [TestMethod]
        public void TestUuid()
        {
            TestIds(IdGenerator.Uuid, 32);
        }
    }
}