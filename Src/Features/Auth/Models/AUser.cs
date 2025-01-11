using AspNetCore.Identity.MongoDbCore.Models;
using HUBT_Social_API.Core.Settings;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace HUBT_Social_API.Features.Auth.Models;

[CollectionName("user")]
[BsonIgnoreExtraElements]
public class AUser : MongoIdentityUser<Guid>
{
    public string AvataUrl { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public Gender Gender { get; set; } = Gender.Other;

    public DateTime DateOfBirth { get; set; }
    public bool IsAuthorticated { get; set; }
    public string FCMToken { get; set; } = string.Empty;
    public string status { get; set; } = string.Empty;

    public AUser()
    {
        AvataUrl = KeyStore.GetRandomAvatarDefault(Gender);
    }
}