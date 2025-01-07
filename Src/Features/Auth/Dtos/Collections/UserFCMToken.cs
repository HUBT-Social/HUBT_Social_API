using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace HUBT_Social_API.Src.Features.Auth.Dtos.Collections
{
    [CollectionName("UserFCMToken")]
    public class UserFCMToken
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId), BsonElement("ID"),] public string Id { get; set; } = string.Empty;
        [BsonElement("userId")] public string UserId { get; set; } = string.Empty;

        [BsonElement("fcmToken")] public string FcmToken { get; set; } = string.Empty;

        [BsonElement("deviceId")] public string DeviceId { get; set; } = string.Empty;
    }
}
