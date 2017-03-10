using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PipServices.Runtime.Portability
{
    [TestClass]
    public class DynamicMapTest
    {
        [TestMethod]
        public void TestSetDefaults()
        {
            var result = DynamicMap.FromValue(new {value1 = 123, value2 = 234});
            result = result.MergeDeep(DynamicMap.FromValue(new {value2 = 432, value3 = 345}));

            Assert.AreEqual(3, result.Keys.Count);
            Assert.AreEqual(123, result["value1"]);
            Assert.AreEqual(234, result["value2"]);
            Assert.AreEqual(345, result["value3"]);
        }

        [TestMethod]
        public void TestSetDefaultsRecursive()
        {
            var result = DynamicMap.FromValue(new {value1 = 123, value2 = new {value21 = 111, value22 = 222}});
            result =
                result.MergeDeep(DynamicMap.FromValue(new {value2 = new {value22 = 777, value23 = 333}, value3 = 345}));

            Assert.AreEqual(3, result.Keys.Count);
            Assert.AreEqual(123, result["value1"]);
            Assert.AreEqual(345, result["value3"]);

            var deepResult = result["value2"] as DynamicMap;
            Assert.AreEqual(3, deepResult.Keys.Count);
            Assert.AreEqual(111, deepResult["value21"]);
            Assert.AreEqual(222, deepResult["value22"]);
            Assert.AreEqual(333, deepResult["value23"]);
        }

        [TestMethod]
        public void TestSetDefaultsWithNulls()
        {
            var result = DynamicMap.FromValue(new {value1 = 123, value2 = 234});

            Assert.AreEqual(2, result.Keys.Count);
            Assert.AreEqual(123, result["value1"]);
            Assert.AreEqual(234, result["value2"]);
        }

        [TestMethod]
        public void TestGet()
        {
            var config = DynamicMap.FromValue(new
            {
                value1 = 123,
                value2 = new
                {
                    value21 = 111,
                    value22 = 222
                }
            });

            var value = config.Get("");
            Assert.IsNull(value);

            value = config.Get("value1");
            Assert.IsNotNull(value);
            Assert.AreEqual(123, value);

            value = config.Get("value2");
            Assert.IsNotNull(value);

            value = config.Get("value3");
            Assert.IsNull(value);

            value = config.Get("value2.value21");
            Assert.IsNotNull(value);
            Assert.AreEqual(111, value);

            value = config.Get("value2.value31");
            Assert.IsNull(value);

            value = config.Get("value2.value21.value211");
            Assert.IsNull(value);

            value = config.Get("valueA.valueB.valueC");
            Assert.IsNull(value);
        }

        [TestMethod]
        public void TestHas()
        {
            var config = DynamicMap.FromValue(new
            {
                value1 = 123,
                value2 = new
                {
                    value21 = 111,
                    value22 = 222
                }
            });

            var has = config.Has("");
            Assert.IsFalse(has);

            has = config.Has("value1");
            Assert.IsTrue(has);

            has = config.Has("value2");
            Assert.IsTrue(has);

            has = config.Has("value3");
            Assert.IsFalse(has);

            has = config.Has("value2.value21");
            Assert.IsTrue(has);

            has = config.Has("value2.value31");
            Assert.IsFalse(has);

            has = config.Has("value2.value21.value211");
            Assert.IsFalse(has);

            has = config.Has("valueA.valueB.valueC");
            Assert.IsFalse(has);
        }

        [TestMethod]
        public void TestSet()
        {
            var config = new DynamicMap();

            config.Set(null, 123);
            Assert.AreEqual(0, config.Count);

            config.Set("field1", 123);
            Assert.AreEqual(1, config.Count);
            Assert.AreEqual(123, config.Get("field1"));

            config.Set("field2", "ABC");
            Assert.AreEqual(2, config.Count);
            Assert.AreEqual("ABC", config.Get("field2"));

            config.Set("field2.field1", 123);
            Assert.AreEqual("ABC", config.Get("field2"));

            config.Set("field3.field31", 456);
            Assert.AreEqual(3, config.Count);
            var subConfig = config.GetNullableMap("field3");
            Assert.IsNotNull(subConfig);
            Assert.AreEqual(456, subConfig.Get("field31"));

            config.Set("field3.field32", "XYZ");
            Assert.AreEqual("XYZ", config.Get("field3.field32"));
        }
    }
}