using HUBT_Social_API.Features.Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace HUBTSOCIAL.Src.Features.Chat.Helpers
{
    public class UserHelper
    {
        private readonly UserManager<AUser> _userManager;

        public UserHelper(UserManager<AUser> userManager)
        {
            Console.WriteLine("Init UserHelper");
            _userManager = userManager;
        }

        public async Task<string> GetFullNameById(string userId)
        {
            AUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null) return "Member"; // Kiểm tra null trước

            Console.WriteLine($"Full name is: {user.FirstName} {user.LastName}");
            return $"{user.FirstName} {user.LastName}";
        }

        public async Task<string?> GetAvatarUserById(string userId) // Sửa Avater -> Avatar
        {
            AUser? user = await _userManager.FindByIdAsync(userId);
            return user?.AvataUrl; // Dùng toán tử `?.` gọn hơn
        }

        public async Task<string?> GetUserIdByUserName(string userName)
        {
            AUser? user = await _userManager.FindByNameAsync(userName);
            return user?.Id.ToString(); // Dùng toán tử `?.` tránh lỗi NullReferenceException
        }
    }
}
