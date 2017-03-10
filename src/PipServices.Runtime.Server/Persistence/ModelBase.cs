using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using PipServices.Runtime.Data;

namespace PipServices.Runtime.Persistence
{
    [DataContract]
    public class ModelBase : IIdentifiable
    {
        [BsonId]
        [BsonIgnoreIfNull]
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DataMember(Name = "id", IsRequired = true)]
        public string Id { get; set; }
    }
}