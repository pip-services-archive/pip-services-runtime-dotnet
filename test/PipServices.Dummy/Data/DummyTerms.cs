using MongoDB.Bson;
using MongoDB.Driver;
using PipServices.Runtime.Data;
using PipServices.Runtime.Persistence;

namespace PipServices.Dummy.Data
{
    public class DummyTerms : Terms<DummyObject>
    {
        public override FilterDefinition<DummyObject> GetFilterDefinition(FilterParams filterParams)
        {
            FilterDefinition<DummyObject> filter = new BsonDocument();

            if (filterParams == null)
                return filter;

            var key = filterParams.GetNullableString("key");

            if (string.IsNullOrWhiteSpace(key))
                return filter;

            var builder = Builders<DummyObject>.Filter;
            filter = filter & builder.Eq(x => x.Key, key);

            return filter;
        }
    }
}