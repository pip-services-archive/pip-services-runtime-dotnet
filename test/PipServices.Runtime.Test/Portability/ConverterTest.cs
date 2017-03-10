using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PipServices.Runtime.Portability
{
    [TestClass]
    public class ConverterTest
    {
        [TestMethod]
        public void TestToString()
        {
            Assert.AreEqual(null, Converter.ToNullableString(null));
            Assert.AreEqual("xyz", Converter.ToString("xyz"));
            Assert.AreEqual("123", Converter.ToString(123));
            Assert.AreEqual("True", Converter.ToString(true));
            Assert.AreEqual("{ prop = xyz }", Converter.ToStringWithDefault(new {prop = "xyz"}, "xyz"));

            Assert.AreEqual("xyz", Converter.ToStringWithDefault(null, "xyz"));
        }

        [TestMethod]
        public void TestToBoolean()
        {
            Assert.IsTrue(Converter.ToBoolean(true));
            Assert.IsTrue(Converter.ToBoolean(1));
            Assert.IsTrue(Converter.ToBoolean("True"));
            Assert.IsTrue(Converter.ToBoolean("yes"));
            Assert.IsTrue(Converter.ToBoolean("1"));
            Assert.IsTrue(Converter.ToBoolean("Y"));

            Assert.IsFalse(Converter.ToBoolean(false));
            Assert.IsFalse(Converter.ToBoolean(0));
            Assert.IsFalse(Converter.ToBoolean("False"));
            Assert.IsFalse(Converter.ToBoolean("no"));
            Assert.IsFalse(Converter.ToBoolean("0"));
            Assert.IsFalse(Converter.ToBoolean("N"));

            Assert.IsFalse(Converter.ToBoolean(123));
            Assert.IsTrue(Converter.ToBooleanWithDefault("XYZ", true));
        }

        [TestMethod]
        public void TestToInteger()
        {
            Assert.AreEqual(123, Converter.ToInteger(123));
            Assert.AreEqual(123, Converter.ToInteger(123.456));
            Assert.AreEqual(123, Converter.ToInteger("123"));

            Assert.AreEqual(123, Converter.ToIntegerWithDefault(null, 123));
            Assert.AreEqual(0, Converter.ToIntegerWithDefault(false, 123));
            Assert.AreEqual(123, Converter.ToIntegerWithDefault("ABC", 123));
        }

        [TestMethod]
        public void TestToFloat()
        {
            Assert.AreEqual(123, Converter.ToFloat(123), 0.001);
            Assert.AreEqual(123.456, Converter.ToFloat(123.456), 0.001);
            Assert.AreEqual(123.456, Converter.ToFloat("123.456"), 0.001);

            Assert.AreEqual(123, Converter.ToFloatWithDefault(null, 123), 0.001);
            Assert.AreEqual(0, Converter.ToFloatWithDefault(false, 123), 0.001);
            Assert.AreEqual(123, Converter.ToFloatWithDefault("ABC", 123), 0.001);
        }

        [TestMethod]
        public void TestToDate()
        {
            Assert.AreEqual(new DateTime(0), Converter.ToDate(null));
            Assert.AreEqual(new DateTime(1975, 4, 8), Converter.ToDateWithDefault(null, new DateTime(1975, 4, 8)));
            Assert.AreEqual(new DateTime(1975, 4, 8), Converter.ToDate(new DateTime(1975, 4, 8)));
            Assert.AreEqual(new DateTime(123456), Converter.ToDate(123456));
            Assert.AreEqual(new DateTime(1975, 4, 8), Converter.ToDate("1975/04/08"));
            Assert.AreEqual(new DateTime(0), Converter.ToDate("XYZ"));
        }

        [TestMethod]
        public void TestFromMultiString()
        {
            Assert.AreEqual(null, Converter.FromMultiString(null));
            Assert.AreEqual("Just a text", Converter.FromMultiString("Just a text"));
            Assert.AreEqual("English text", Converter.FromMultiString(new {en = "English text", ru = "Russian text"}));
            Assert.AreEqual("Russian text",
                Converter.FromMultiString(new {en = "English text", ru = "Russian text"}, "ru"));
            Assert.AreEqual("Russian text", Converter.FromMultiString(new {ru = "Russian text", sp = "Spanish text"}));
        }

        [TestMethod]
        public void TestToMap()
        {
            // Handling nulls
            object value = null;
            IDictionary<string, object> result = Converter.ToNullableMap(value);
            Assert.IsNull(result);

            // Handling simple objects
            value = new {value1 = 123, value2 = 234};
            result = Converter.ToMap(value);
            Assert.IsNotNull(result);
            Assert.AreEqual(123, result["value1"]);
            Assert.AreEqual(234, result["value2"]);

            // Handling dictionaries
            value = new DynamicMap();
            result = Converter.ToMap(value);
            Assert.IsTrue(value == result);

            // Recursive conversion
            value = new {value1 = 123, value2 = new {value21 = 111, value22 = 222}};
            result = Converter.ToNullableMap(value);
            Assert.IsNotNull(result);
            Assert.AreEqual(123, result["value1"]);
            Assert.IsNotNull(result["value2"]);
            Assert.IsTrue(result["value2"] is IDictionary<string, object>);

            // Handling arrays
            value = new {value1 = new object[] {new {value11 = 111, value12 = 222}}};
            result = Converter.ToNullableMap(value);
            Assert.IsNotNull(result);
            Assert.IsTrue(result["value1"] is IEnumerable<object>);
            var resultElements = (result["value1"] as IEnumerable<object>).ToArray();
            var resultElement0 = resultElements[0] as IDictionary<string, object>;
            Assert.IsNotNull(resultElement0);
            Assert.AreEqual(111, resultElement0["value11"]);
            Assert.AreEqual(222, resultElement0["value12"]);

            // Handling lists
            value = new {value1 = new List<object>(new object[] {new {value11 = 111, value12 = 222}})};
            result = Converter.ToNullableMap(value);
            Assert.IsNotNull(result);
            Assert.IsTrue(result["value1"] is IEnumerable<object>);
            resultElements = (result["value1"] as IEnumerable<object>).ToArray();
            resultElement0 = resultElements[0] as IDictionary<string, object>;
            Assert.IsNotNull(resultElement0);
            Assert.AreEqual(111, resultElement0["value11"]);
            Assert.AreEqual(222, resultElement0["value12"]);
        }
    }
}