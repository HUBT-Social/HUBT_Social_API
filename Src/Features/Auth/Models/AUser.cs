using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace HUBT_Social_API.Features.Auth.Models;

[CollectionName("user")]
[BsonIgnoreExtraElements]
public class AUser : MongoIdentityUser<Guid>
{
    public string StudentImageURL { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public bool IsMale { get; set; } = false;

    public DateTime DateOfBirth { get; set; }
}