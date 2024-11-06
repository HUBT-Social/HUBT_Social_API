using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace HUBT_Social_API.Features.Auth.Models;

[CollectionName("user")]
public class AUser : MongoIdentityUser<Guid>
{
    public string StudentImageURL { get; set; } = string.Empty;
    public string DefaultUserImage { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public bool IsMale { get; set; } = false;

    public DateTime DateOfBirth { get; set; }

    public AUser()
    {
        DefaultUserImage = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1730901747/v5elptamoonvux5xth0a.jpg";
    }
}