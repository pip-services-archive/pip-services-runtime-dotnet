using System;
using System.Collections.Generic;
using System.ComponentModel;
using PipServices.Runtime.Data.Mapper;

namespace PipServices.Runtime.Portability
{
    public class DynamicMap : Dictionary<string, object>
    {
        public DynamicMap()
        {
        }

        public DynamicMap(IDictionary<string, object> map)
        {
            AddAll(map);
        }

        /********** Getters ************/

        public T Get<T>(string path)
            where T : class
        {
            return ObjectMapper.MapTo<T>(Get(path));
        }

        public object Get(string path)
        {
            if (path == null) return null;

            var props = path.Split('.');
            object result = this;

            foreach (var prop in props)
            {
                var obj = result as IDictionary<string, object>;

                if (obj == null || !obj.ContainsKey(prop))
                    return null;

                result = obj[prop];
            }

            return result;
        }

        public bool Has(string path)
        {
            return Get(path) != null;
        }

        public bool HasNot(string path)
        {
            return Get(path) == null;
        }

        public DynamicMap GetNullableMap(string path)
        {
            var value = Get(path);
            return Converter.ToNullableMap(value);
        }

        public DynamicMap GetMap(string path)
        {
            var value = Get(path);
            return Converter.ToNullableMap(value) ?? new DynamicMap();
        }

        public DynamicMap GetMapWithDefault(string path, DynamicMap defaultValue = null)
        {
            var value = Get(path);
            return Converter.ToMapWithDefault(value, defaultValue);
        }

        public IList<object> GetNullableArray(string path)
        {
            var value = Get(path);
            return Converter.ToNullableArray(value);
        }

        public IList<object> GetArray(string path)
        {
            var value = GetNullableArray(path);
            return value != null ? value : new List<object>();
        }

        public IList<object> GetArrayWithDefault(string path, IList<object> defaultValue)
        {
            var value = GetNullableArray(path);
            return value != null ? value : defaultValue;
        }

        public string GetNullableString(string path)
        {
            var value = Get(path);
            return Converter.ToNullableString(value);
        }

        public string GetString(string path)
        {
            return GetStringWithDefault(path, "");
        }

        public string GetStringWithDefault(string path, string defaultValue = null)
        {
            var value = Get(path);
            return Converter.ToStringWithDefault(value, defaultValue);
        }

        public bool? GetNullableBoolean(string path)
        {
            var value = Get(path);
            return Converter.ToNullableBoolean(value);
        }

        public bool GetBoolean(string path)
        {
            return GetBooleanWithDefault(path, false);
        }

        public bool GetBooleanWithDefault(string path, bool defaultValue = false)
        {
            var value = Get(path);
            return Converter.ToBooleanWithDefault(value, defaultValue);
        }

        public int? GetNullableInteger(string path)
        {
            var value = Get(path);
            return Converter.ToNullableInteger(value);
        }

        public int GetInteger(string path)
        {
            return GetIntegerWithDefault(path, 0);
        }

        public int GetIntegerWithDefault(string path, int defaultValue = 0)
        {
            var value = Get(path);
            return Converter.ToIntegerWithDefault(value, defaultValue);
        }

        public long? GetNullableLong(string path)
        {
            var value = Get(path);
            return Converter.ToNullableLong(value);
        }

        public long GetLong(string path)
        {
            return GetLongWithDefault(path, 0);
        }

        public long GetLongWithDefault(string path, long defaultValue = 0)
        {
            var value = Get(path);
            return Converter.ToLongWithDefault(value, defaultValue);
        }

        public float? GetNullableFloat(string path)
        {
            var value = Get(path);
            return Converter.ToNullableFloat(value);
        }

        public float GetFloat(string path)
        {
            return GetFloatWithDefault(path, 0);
        }

        public float GetFloatWithDefault(string path, float defaultValue = 0)
        {
            var value = Get(path);
            return Converter.ToFloatWithDefault(value, defaultValue);
        }

        public DateTime? GetNullableDate(string path)
        {
            var value = Get(path);
            return Converter.ToNullableDate(value);
        }

        public DateTime GetDate(string path)
        {
            return GetDateWithDefault(path, new DateTime(0));
        }

        public DateTime GetDateWithDefault(string path, DateTime? defaultValue = null)
        {
            var value = Get(path);
            return Converter.ToDateWithDefault(value, defaultValue);
        }

        /*********** Setters ***********/

        public void Set(string path, object value)
        {
            if (path == null) return;

            var props = path.Split('.');
            if (props.Length == 0) return;

            IDictionary<string, object> container = this;

            for (var i = 0; i < props.Length - 1; i++)
            {
                var prop = props[i];

                object obj = null;
                container.TryGetValue(prop, out obj);
                if (obj == null)
                {
                    IDictionary<string, object> temp = new DynamicMap();
                    container[prop] = temp;
                    container = temp;
                }
                else
                {
                    if (!(obj is IDictionary<string, object>))
                        return;

                    container = (IDictionary<string, object>) obj;
                }
            }

            container[props[props.Length - 1]] = value;
        }

        public void AddAll(IDictionary<string, object> map)
        {
            foreach (var key in map.Keys)
            {
                this[key] = map[key];
            }
        }

        public void SetTuplesArray(object[] values)
        {
            for (var i = 0; i < values.Length; i += 2)
            {
                if (i + 1 >= values.Length) break;

                var path = Converter.ToString(values[i]);
                var propValue = values[i + 1];

                Set(path, propValue);
            }
        }

        public void SetAll(params object[] values)
        {
            SetTuplesArray(values);
        }

        //public override void Remove(string path)
        //{
        //    // Todo: implement hierarchical delete
        //    base.remove(path);
        //}

        public void RemoveAll(params string[] path)
        {
            foreach (var prop in path)
            {
                Remove(prop);
            }
        }

        /********** Merging ***********/

        public static DynamicMap Merge(DynamicMap dest, IDictionary<string, object> source, bool deep)
        {
            if (dest == null) dest = new DynamicMap();
            if (source == null) return dest;

            foreach (var key in source.Keys)
            {
                if (dest.ContainsKey(key))
                {
                    var configValue = dest[key];
                    var defaultValue = source[key];

                    if (deep && configValue is IDictionary<string, object>
                        && defaultValue is IDictionary<string, object>)
                    {
                        dest[key] = Merge(
                            new DynamicMap((IDictionary<string, object>) configValue),
                            (IDictionary<string, object>) defaultValue,
                            deep
                            );
                    }
                }
                else
                {
                    dest[key] = source[key];
                }
            }

            return dest;
        }

        public static DynamicMap SetDefaults(DynamicMap dest, IDictionary<string, object> source)
        {
            return Merge(dest, source, true);
        }

        public DynamicMap Merge(IDictionary<string, object> source, bool deep)
        {
            var dest = new DynamicMap(this);
            return Merge(dest, source, deep);
        }

        public DynamicMap MergeDeep(IDictionary<string, object> source)
        {
            return Merge(source, true);
        }

        public DynamicMap SetDefaults(IDictionary<string, object> defaults)
        {
            return Merge(defaults, true);
        }

        /********** Other Utilities *********/

        public void AssignTo(object value)
        {
            if (value == null || Count == 0) return;

            var props = TypeDescriptor.GetProperties(value);
            foreach (var entry in this)
            {
                var prop = props.Find(entry.Key, true);
                if (prop != null) prop.SetValue(value, entry.Value);
            }
        }

        public DynamicMap Pick(params string[] paths)
        {
            var result = new DynamicMap();
            foreach (var path in paths)
            {
                object value = null;
                if (TryGetValue(path, out value))
                    result.Add(path, value);
            }
            return result;
        }

        public DynamicMap Omit(params string[] paths)
        {
            var result = new DynamicMap(this);
            foreach (var path in paths)
            {
                result.Remove(path);
            }
            return result;
        }

        /********* Class constructors ********/

        public static DynamicMap FromValue(object value)
        {
            return Converter.ToMap(value);
        }

        public static DynamicMap FromTuples(params object[] tuples)
        {
            var result = new DynamicMap();
            result.SetTuplesArray(tuples);
            return result;
        }
    }
}