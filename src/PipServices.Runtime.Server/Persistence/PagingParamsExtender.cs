using MongoDB.Driver;
using PipServices.Runtime.Data;

namespace PipServices.Runtime.Persistence
{
    public static class PagingParamsExtender
    {
        public static IFindFluent<T, T> ModifyFinder<T>(this PagingParams paging, IFindFluent<T, T> finder)
        {
            if (paging.HasTotal())
                return finder;

            var localFinder = finder;

            if (paging.Skip.HasValue)
                localFinder = localFinder.Skip(paging.Skip);

            if (paging.Take.HasValue)
                localFinder = localFinder.Limit(paging.Take);

            return localFinder;
        }
    }
}