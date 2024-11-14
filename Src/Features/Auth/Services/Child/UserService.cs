using HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;

namespace HUBT_Social_API.Features.Auth.Services.Child;

public class UserService : IUserService
{
    private readonly RoleManager<ARole> _roleManager;
    private readonly UserManager<AUser> _userManager;
    // Define a dictionary to rank roles
    private readonly Dictionary<string, int> _roleHierarchy = new Dictionary<string, int>
    {
        { "ADMIN", 4 },
        { "HEAD", 3 },
        { "TEACHER", 2 },
        { "STUDENT", 1 },
        { "USER", 0}
    };

    public UserService(UserManager<AUser> userManager, RoleManager<ARole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<AUser?> FindUserByUserNameAsync(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return null;
        return await _userManager.FindByNameAsync(userName);
    }
    public async Task<AUser?> FindUserByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;
        return await _userManager.FindByEmailAsync(email);
    }
    

    public async Task<bool> PromoteUserAccountAsync(string currentUserName, string targetUserName, string roleName)
    {
        // Validate the role name
        if (!IsValidRole(roleName) || string.IsNullOrEmpty(targetUserName))
            return false;

        // Get the current user and target user
        var currentUser = await _userManager.FindByNameAsync(currentUserName);
        var targetUser = await _userManager.FindByNameAsync(targetUserName);
        if (currentUser == null || targetUser == null)
            return false;

        // Get roles of current and target users
        var currentUserRoles = await _userManager.GetRolesAsync(currentUser);
        var targetUserRoles = await _userManager.GetRolesAsync(targetUser);

        // Get highest role of each user
        var currentUserHighestRole = GetHighestRole(currentUserRoles);
        var targetUserHighestRole = GetHighestRole(targetUserRoles);

        // Check if current user has a higher role than target user
        if (_roleHierarchy[currentUserHighestRole] <= _roleHierarchy[targetUserHighestRole])
            return false; // Current user doesn't have a higher role

        // Ensure role exists, then add role to target user
        await EnsureRoleExistsAsync(roleName);
        var result = await _userManager.AddToRoleAsync(targetUser, roleName);

        return result.Succeeded;
    }

    // Method to get the highest role of a user based on hierarchy
    private string GetHighestRole(IList<string> roles)
    {
        return roles.OrderByDescending(role => _roleHierarchy.GetValueOrDefault(role, 0)).FirstOrDefault();
    }

    // Existing helper methods
    private bool IsValidRole(string roleName) => _roleHierarchy.ContainsKey(roleName);

    private async Task EnsureRoleExistsAsync(string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            var role = new ARole { Name = roleName };
            await _roleManager.CreateAsync(role);
        }
    }

    public async Task<bool> UpdateEmailAsync(string userName, UpdateEmailRequest request)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null) return false;

        user.Email = request.Email;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> VerifyCurrentPasswordAsync(string userName, CheckPasswordRequest request)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null) return false;

        // Kiểm tra mật khẩu hiện tại
        var passwordCheck = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
        return passwordCheck; // Trả về true nếu mật khẩu đúng, ngược lại là false
    }

    public async Task<bool> UpdatePasswordAsync(string userName, UpdatePasswordRequest request)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null) return false;

        // Tạo token đặt lại mật khẩu
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        // Đặt lại mật khẩu với token và mật khẩu mới
        var result = await _userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);
        return result.Succeeded;
    }

    public async Task<bool> UpdateNameAsync(string userName, UpdateNameRequest request)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null) return false;

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> UpdatePhoneNumberAsync(string userName, UpdatePhoneNumberRequest request)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null) return false;

        user.PhoneNumber = request.PhoneNumber;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> UpdateGenderAsync(string userName, UpdateGenderRequest request)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null) return false;

        user.IsMale = request.IsMale;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> UpdateDateOfBirthAsync(string userName, UpdateDateOfBornRequest request)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null) return false;

        user.DateOfBirth = request.DateOfBirth;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> GeneralUpdateAsync(string userName, GeneralUpdateRequest request)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null) return false;

        // Cập nhật các trường nếu có thay đổi
        if (!string.IsNullOrEmpty(request.Email))
            user.Email = request.Email;

        if (!string.IsNullOrEmpty(request.FirstName))
            user.FirstName = request.FirstName;

        if (!string.IsNullOrEmpty(request.LastName))
            user.LastName = request.LastName;

        if (!string.IsNullOrEmpty(request.PhoneNumber))
            user.PhoneNumber = request.PhoneNumber;

        user.IsMale = request.IsMale;

        if (request.DateOfBirth != DateTime.MinValue)
            user.DateOfBirth = request.DateOfBirth;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> EnableTwoFactor(string userName)
    {
        AUser? user = await _userManager.FindByNameAsync(userName);
        if (user == null) return false;

        user.TwoFactorEnabled = true;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded) return false;

        return true;
    }

    public async Task<bool> DisableTwoFactor(string userName)
    {

        AUser? user = await _userManager.FindByNameAsync(userName);
        if (user == null) return false;

        user.TwoFactorEnabled = false;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded) return false;

        return true;
    }

}