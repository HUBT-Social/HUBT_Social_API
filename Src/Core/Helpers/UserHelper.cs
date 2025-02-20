using HUBT_Social_API.Features.Auth.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace HUBTSOCIAL.Src.Features.Chat.Helpers
{
    public static class UserHelper
    {
        public static async Task<string> GetFullNameById(UserManager<AUser> userManager, string userId)
        {
            AUser? user = await userManager.FindByIdAsync(userId);
            return user == null ? "Member" : $"{user.FirstName} {user.LastName}";
        }

        public static async Task<string?> GetAvatarUserById(UserManager<AUser> userManager, string userId)
        {
            AUser? user = await userManager.FindByIdAsync(userId);
            return user?.AvataUrl;
        }

        public static async Task<string?> GetUserIdByUserName(UserManager<AUser> userManager, string userName)
        {
            AUser? user = await userManager.FindByNameAsync(userName);
            return user?.Id.ToString();
        }
    }
}
