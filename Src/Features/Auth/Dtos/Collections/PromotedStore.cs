using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace HUBT_Social_API.Src.Features.Auth.Dtos.Collections
{
    [CollectionName("PromotedStore")]
    public class PromotedStore
    {
        [BsonId, BsonElement("userId")] public string UserId { get; set; } = string.Empty;

        [BsonElement("identifyCode")] public string IdentifyCode { get; set; } = string.Empty;

        [BsonElement("email")] public string email { get; set; } = string.Empty;

        [BsonElement("ExpireTime")]
        [BsonDateTimeOptions]
        public DateTime ExpireTime { get; set; }
    }
}
