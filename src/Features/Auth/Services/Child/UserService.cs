using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace HUBT_Social_API.Features.Auth.Services.Child;

public class UserService : IUserService
{
    private readonly RoleManager<ARole> _roleManager;
    private readonly UserManager<AUser> _userManager;

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
            throw new ArgumentNullException(nameof(userName), "Tên người dùng không thể null hoặc rỗng.");

        if (string.IsNullOrEmpty(roleName) ||
            (roleName != "USER" && roleName != "TEACHER" && roleName != "HEAD" && roleName != "ADMIN"))
            throw new ArgumentException("Giá trị vai trò không hợp lệ.", nameof(roleName));

        var user = await _userManager.FindByNameAsync(userName);
        if (user != null)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var role = new ARole { Name = roleName };
                await _roleManager.CreateAsync(role);
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            return result.Succeeded;
        }

        return false;
    }


    // Tương tự cho các phương thức PromoteToTeacherAsync, VerifyCodeAsync, v.v.
}