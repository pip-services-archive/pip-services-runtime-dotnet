using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using PipServices.Runtime.Data;

namespace PipServices.Runtime.Portability
{
    public static class Converter
    {
        public static string ToNullableString(object value)
        {
            if (value == null) return null;
            if (value is string) return (string) value;
            if (value is DateTime) ((DateTime) value).ToString("o", CultureInfo.InvariantCulture);
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        public static string ToString(object value)
        {
            return ToStringWithDefault(value, "");
        }

        public static string ToStringWithDefault(object value, string defaultValue = null)
        {
            if (value == null) return defaultValue;
            if (value is string) return (string) value;
            if (value is DateTime) ((DateTime) value).ToString("o", CultureInfo.InvariantCulture);
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        public static bool? ToNullableBoolean(object value)
        {
            if (value == null) return null;
            if (value is bool) return (bool) value;

            var strValue = Convert.ToString(value, CultureInfo.InvariantCulture).ToLowerInvariant();
            if (strValue == "1" || strValue == "true" || strValue == "t"
                || strValue == "yes" || strValue == "y")
                return true;

            if (strValue == "0" || strValue == "false" || strValue == "f"
                || strValue == "no" || strValue == "n")
                return false;

            return null;
        }

        public static bool ToBoolean(object value)
        {
            return ToBooleanWithDefault(value, false);
        }

        public static bool ToBooleanWithDefault(object value, bool defaultValue = false)
        {
            var result = ToNullableBoolean(value);
            return result.HasValue ? result.Value : defaultValue;
        }

        public static int? ToNullableInteger(object value)
        {
            if (value == null) return null;
            if (value is DateTime) return (int) ((DateTime) value).Ticks;
            if (value is bool) return (bool) value ? 1 : 0;

            try
            {
                return Convert.ToInt32(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        public static int ToInteger(object value)
        {
            return ToIntegerWithDefault(value, 0);
        }

        public static int ToIntegerWithDefault(object value, int defaultValue = 0)
        {
            var result = ToNullableInteger(value);
            return result.HasValue ? result.Value : defaultValue;
        }

        public static long? ToNullableLong(object value)
        {
            if (value == null) return null;
            if (value is DateTime) return ((DateTime) value).Ticks;
            if (value is bool) return (bool) value ? 1 : 0;

            try
            {
                return Convert.ToInt64(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        public static long ToLong(object value)
        {
            return ToLongWithDefault(value, 0);
        }

        public static long ToLongWithDefault(object value, long defaultValue = 0)
        {
            var result = ToNullableLong(value);
            return result.HasValue ? result.Value : defaultValue;
        }

        public static float? ToNullableFloat(object value)
        {
            if (value == null) return null;
            if (value is DateTime) return ((DateTime) value).Ticks;
            if (value is bool) return (bool) value ? 1 : 0;

            try
            {
                return Convert.ToSingle(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        public static float ToFloat(object value)
        {
            return ToFloatWithDefault(value, 0);
        }

        public static float ToFloatWithDefault(object value, float defaultValue = 0)
        {
            var result = ToNullableFloat(value);
            return result.HasValue ? result.Value : defaultValue;
        }

        public static DateTime? ToNullableDate(object value)
        {
            if (value == null) return null;
            if (value is DateTime) return (DateTime) value;
            if (value is int) return new DateTime((int) value);
            if (value is long) return new DateTime((long) value);

            try
            {
                if (value is string)
                    return DateTime.Parse((string) value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

                return Convert.ToDateTime(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        public static DateTime ToDate(object value)
        {
            return ToDateWithDefault(value, new DateTime(0));
        }

        public static DateTime ToDateWithDefault(object value, DateTime? defaultValue = null)
        {
            var realDefault = defaultValue.HasValue ? defaultValue.Value : new DateTime(0);
            var result = ToNullableDate(value);
            return result.HasValue ? result.Value : realDefault;
        }

        public static IList<object> ToNullableArray(object value)
        {
            // Return null when nothing found
            if (value == null)
            {
                return null;
            }
                // Convert list
            if (value is IList<object>)
            {
                return (IList<object>) value;
            }
                // Convert enumerable
            if (value is IEnumerable<object>)
            {
                return new List<object>((IEnumerable<object>) value);
            }
                // Convert single values
            IList<object> array = new List<object>();
            array.Add(value);
            return array;
        }

        public static IList<object> ToArray(object value)
        {
            var result = ToNullableArray(value);
            return result != null ? result : new List<object>();
        }

        public static IList<object> ToArrayWithDefault(object value, IList<object> defaultValue)
        {
            var result = ToNullableArray(value);
            return result != null ? result : defaultValue;
        }

        public static string FromMultiString(object value, string language = "en")
        {
            if (value == null) return null;
            if (value is string) return (string) value;

            IDictionary<string, object> values = ToNullableMap(value);

            string result;

            if (values.ContainsKey(language))
            {
                result = ToString(values[language]);
                if (result != null && result.Length > 0) return result;
            }

            if (values.ContainsKey("en"))
            {
                result = ToString(values["en"]);
                if (result != null && result.Length > 0) return result;
            }

            foreach (var entry in values)
            {
                result = ToString(entry.Value);
                if (result != null && result.Length > 0) return result;
            }

            return null;
        }

        private static DynamicMap ObjectToMap(object value)
        {
            if (value == null) return null;

            var result = new DynamicMap();
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(value))
            {
                var propValue = prop.GetValue(value);

                // Recursive conversion
                propValue = ValueToMap(propValue);

                result.Add(prop.Name, propValue);
            }
            return result;
        }

        private static object[] ArrayToMap(IEnumerable<object> value)
        {
            var result = value as object[] ?? value.ToArray();

            for (var index = 0; index < result.Length; index++)
                result[index] = ValueToMap(result[index]);

            return result;
        }

        private static DynamicMap MapToMap(IDictionary<string, object> value)
        {
            var result = new DynamicMap();

            foreach (var key in value.Keys)
                result[key] = ValueToMap(value[key]);

            return result;
        }

        private static DynamicMap ObjectMapToMap(IDictionary<object, object> value)
        {
            var result = new DynamicMap();

            foreach (string key in value.Keys)
                result[key] = ValueToMap(value[key]);

            return result;
        }

        private static object ExtensionToMap(object value)
        {
            if (value == null) return null;

            var valueType = value.GetType().Name;

            // Convert extension objects
            if (valueType == "ExtensionDataObject")
            {
                var extResult = new DynamicMap();

                var membersProperty = typeof(ExtensionDataObject).GetProperty(
                    "Members", BindingFlags.NonPublic | BindingFlags.Instance);
                var members = (IList) membersProperty.GetValue(value, null);

                foreach (var member in members)
                {
                    var memberNameProperty = member.GetType().GetProperty("Name");
                    var memberName = (string) memberNameProperty.GetValue(member, null);

                    var memberValueProperty = member.GetType().GetProperty("Value");
                    var memberValue = memberValueProperty.GetValue(member, null);
                    memberValue = ExtensionToMap(memberValue);

                    extResult.Add(memberName, memberValue);
                }

                return extResult;
            }

            // Convert classes
            if (valueType.StartsWith("ClassDataNode"))
            {
                var classResult = new DynamicMap();

                var membersProperty = value.GetType().GetProperty(
                    "Members", BindingFlags.NonPublic | BindingFlags.Instance);
                var members = (IList) membersProperty.GetValue(value, null);

                foreach (var member in members)
                {
                    var memberNameProperty = member.GetType().GetProperty("Name");
                    var memberName = (string) memberNameProperty.GetValue(member, null);

                    var memberValueProperty = member.GetType().GetProperty("Value");
                    var memberValue = memberValueProperty.GetValue(member, null);
                    memberValue = ExtensionToMap(memberValue);

                    classResult.Add(memberName, memberValue);
                }

                return classResult;
            }

            // Convert collections and arrays
            if (valueType.StartsWith("CollectionDataNode"))
            {
                var itemsProperty = value.GetType().GetProperty(
                    "Items", BindingFlags.NonPublic | BindingFlags.Instance);
                var items = (IList) itemsProperty.GetValue(value, null);

                var arrayResult = new object[items.Count];

                for (var index = 0; index < items.Count; index++)
                    arrayResult[index] = ExtensionToMap(items[index]);

                return arrayResult;
            }

            // Convert values
            if (valueType.StartsWith("DataNode"))
            {
                var dataValueProperty = value.GetType().GetProperty("Value");
                var valueResult = dataValueProperty.GetValue(value, null);
                valueResult = ExtensionToMap(valueResult);
                return valueResult;
            }

            return value;
        }

        private static object ValueToMap(object value)
        {
            if (value == null) return null;

            // Skip converted values
            if (value is DynamicMap) return value;

            // Skip expected non-primitive values
            if (value is string || value is Type) return value;

            var valueType = value.GetType();

            // Skip primitive values
            if (valueType.IsPrimitive || valueType.IsValueType) return value;
            // Skip Json.Net values
            if (valueType.Name == "JValue") return value;

            if (value is IDictionary<string, object>)
                return MapToMap((IDictionary<string, object>) value);

            if (value is IDictionary<object, object>)
                return ObjectMapToMap((IDictionary<object, object>) value);

            // Convert arrays
            if (value is IEnumerable<object> && valueType.Name != "JObject")
                return ArrayToMap((IEnumerable<object>) value);

            // Convert partial updates
            if (value is PartialUpdates)
                return ExtensionToMap(((PartialUpdates) value).ExtensionData);

            return ObjectToMap(value);
        }

        public static DynamicMap ToNullableMap(object value)
        {
            return ValueToMap(value) as DynamicMap;
        }

        public static DynamicMap ToMap(object value)
        {
            return ValueToMap(value) as DynamicMap ?? new DynamicMap();
        }

        public static DynamicMap ToMapWithDefault(object value, DynamicMap defaultValue = null)
        {
            return ValueToMap(value) as DynamicMap ?? defaultValue;
        }
    }
}