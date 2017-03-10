using System.Collections.Generic;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Data
{
    public class FilterParams : DynamicMap
    {
        public FilterParams()
        {
        }

        public FilterParams(Dictionary<string, object> map)
            : base(map)
        {
        }

        public new static FilterParams FromValue(object value)
        {
            if (value is FilterParams)
                return (FilterParams) value;
            if (value is DynamicMap)
                return FromMap((DynamicMap) value);

            var map = DynamicMap.FromValue(value);
            return FromMap(map);
        }

        public new static FilterParams FromTuples(params object[] tuples)
        {
            var filter = new FilterParams();
            filter.SetTuplesArray(tuples);
            return filter;
        }

        public static FilterParams FromMap(DynamicMap map)
        {
            return new FilterParams(map);
        }
    }
}