using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using AspNetCore.Identity.MongoDbCore.Models;
using HUBT_Social_API.src.Features.Authentication.Models;
using HUBT_Social_API.src.Features.Auth.Services.IAuthServices;
using HUBT_Social_API.src.Features.Auth.Dtos.Request;

namespace HUBT_Social_API.src.Features.Auth.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AUser> _userManager;
        private readonly RoleManager<ARole> _roleManager;

        public UserService(UserManager<AUser> userManager, RoleManager<ARole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<AUser> FindUserByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Tên người dùng không được để trống.", nameof(userName));

            #pragma warning disable CS8603 
            return await _userManager.FindByNameAsync(userName);
            #pragma warning restore CS8603 
        }
        public async Task<bool> PromoteUserAccountAsync(string userName, string roleName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException(nameof(userName), "Tên người dùng không thể null hoặc rỗng.");
            }

            if (string.IsNullOrEmpty(roleName) || 
                (roleName != "USER" && roleName != "TEACHER" && roleName != "HEAD" && roleName != "ADMIN"))
            {
                throw new ArgumentException("Giá trị vai trò không hợp lệ.", nameof(roleName));
            }

            AUser? user = await _userManager.FindByNameAsync(userName);
            if (user != null)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    ARole? role = new ARole { Name = roleName};
                    await _roleManager.CreateAsync(role);
                }

                IdentityResult? result = await _userManager.AddToRoleAsync(user, roleName);
                return result.Succeeded;
            }

            return false;
        }

        public async Task<bool> ChangeLanguage(ChangeLanguageRequest changeLanguageRequest)
        {
            if (string.IsNullOrWhiteSpace(changeLanguageRequest.Language) || string.IsNullOrWhiteSpace(changeLanguageRequest.UserName))
            {
                throw new ArgumentException("Tên người dùng và ngôn ngữ không được để trống.", nameof(changeLanguageRequest));
            }

            if (changeLanguageRequest.Language != "vi" && changeLanguageRequest.Language != "en")
            {
                return false; // Ngôn ngữ không hợp lệ
            }

            AUser? user = await _userManager.FindByNameAsync(changeLanguageRequest.UserName);
            if (user is null)
            {
                return false; // Không tìm thấy người dùng
            }

            user.Language = changeLanguageRequest.Language;
            try
            {
                await _userManager.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating language: {ex.Message}");
                return false; 
            }
            return true;
        }

        // Tương tự cho các phương thức PromoteToTeacherAsync, VerifyCodeAsync, v.v.
    }
}
