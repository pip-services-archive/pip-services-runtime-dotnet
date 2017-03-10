using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Data
{
    public class PagingParams
    {
        public PagingParams()
        {
        }

        public PagingParams(object skip, object take, object total)
        {
            Skip = Converter.ToNullableInteger(skip);
            Take = Converter.ToNullableInteger(take);
            Total = Converter.ToNullableBoolean(total);
        }

        public int? Skip { get; }
        public int? Take { get; }
        public bool? Total { get; }

        public int GetSkip(int minSkip)
        {
            if (Skip.HasValue == false) return minSkip;
            if (Skip.Value < minSkip) return minSkip;
            return Skip.Value;
        }

        public int GetTake(int maxTake)
        {
            if (Take.HasValue == false) return maxTake;
            if (Take.Value < 0) return 0;
            if (Take.Value > maxTake) return maxTake;
            return Take.Value;
        }

        public bool HasTotal()
        {
            return Total.HasValue ? Total.Value : false;
        }

        public static PagingParams FromValue(object value)
        {
            if (value is PagingParams)
                return (PagingParams) value;
            if (value is DynamicMap)
                return FromMap((DynamicMap) value);

            var map = DynamicMap.FromValue(value);
            return FromMap(map);
        }

        public static PagingParams FromTuples(params object[] tuples)
        {
            var map = DynamicMap.FromTuples(tuples);
            return FromMap(map);
        }

        public static PagingParams FromMap(DynamicMap map)
        {
            var skip = map.GetNullableInteger("skip");
            var take = map.GetNullableInteger("take");
            var total = map.GetNullableBoolean("total");
            return new PagingParams(skip, take, total);
        }
    }
}