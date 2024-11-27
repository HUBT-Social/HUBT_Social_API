using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace HUBT_Social_API.Features.Auth.Dtos.Collections;

[CollectionName("Postcode")]
public class Postcode
{
    [BsonId, BsonRepresentation(BsonType.ObjectId), BsonElement("ID"),] public string Id { get; set; } = string.Empty;
    [BsonElement("UserAgent")] public string UserAgent { get; set; } = string.Empty;
    [BsonElement("IPAddress")] public string IPAddress { get; set; } = string.Empty;
    [BsonElement("Email")] public string Email { get; set; } = string.Empty;
    [BsonElement("Code")] public string Code { get; set; } = string.Empty;
    [BsonElement("PostcodeType")] public string PostcodeType { get; set; } = string.Empty;

    [BsonElement("ExpireTime"),BsonDateTimeOptions]
    public DateTime ExpireTime { get; set; }
}