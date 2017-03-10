using MongoDB.Driver;
using PipServices.Runtime.Data;

namespace PipServices.Runtime.Persistence
{
    public abstract class Terms<T> where T : IIdentifiable
    {
        public abstract FilterDefinition<T> GetFilterDefinition(FilterParams filter);
    }
}