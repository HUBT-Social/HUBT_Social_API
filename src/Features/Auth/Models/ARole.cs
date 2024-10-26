using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace HUBT_Social_API.src.Features.Authentication.Models
{
    [CollectionName("role")]
    public class ARole : MongoIdentityRole<Guid>
    {

    }
}
