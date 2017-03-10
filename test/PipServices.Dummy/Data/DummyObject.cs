using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using PipServices.Runtime.Persistence;

namespace PipServices.Dummy.Data
{
    [DataContract]
    [JsonObject(MemberSerialization.OptIn)]
    [BsonIgnoreExtraElements]
    public class DummyObject : ModelBase
    {
        [BsonRequired]
        [BsonElement("key")]
        [JsonProperty("key", DefaultValueHandling = DefaultValueHandling.Include)]
        [DataMember(Name = "key", IsRequired = true)]
        public string Key { get; set; }

        [BsonRequired]
        [BsonElement("content")]
        [JsonProperty("content", DefaultValueHandling = DefaultValueHandling.Include)]
        [DataMember(Name = "content", IsRequired = false)]
        public string Content { get; set; }
    }
}