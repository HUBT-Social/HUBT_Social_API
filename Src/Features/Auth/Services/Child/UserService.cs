using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace HUBT_Social_API.Features.Auth.Services.Child;

public class UserService : IUserService
{
    private readonly RoleManager<ARole> _roleManager;
    private readonly UserManager<AUser> _userManager;
    private readonly ILogger<UserService> _logger;

    private readonly Dictionary<string, int> _roleHierarchy = new Dictionary<string, int>
    {
        { "ADMIN", 4 },
        { "HEAD", 3 },
        { "TEACHER", 2 },
        { "STUDENT", 1 },
        { "USER", 0 }
    };

    public UserService(UserManager<AUser> userManager, RoleManager<ARole> roleManager, ILogger<UserService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    private async Task<AUser?> GetUserByNameAsync(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName)) return null;
        return await _userManager.FindByNameAsync(userName);
    }

    public async Task<AUser?> FindUserByUserNameAsync(string userName) => await GetUserByNameAsync(userName);

    public async Task<AUser?> FindUserByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<bool> PromoteUserAccountAsync(string currentUserName, string targetUserName, string roleName)
    {
        try
        {
            if (!IsValidRole(roleName) || string.IsNullOrEmpty(targetUserName)) return false;

            var currentUser = await GetUserByNameAsync(currentUserName);
            var targetUser = await GetUserByNameAsync(targetUserName);
            if (currentUser == null || targetUser == null) return false;

            var currentUserRoles = await _userManager.GetRolesAsync(currentUser);
            var targetUserRoles = await _userManager.GetRolesAsync(targetUser);

            var currentUserHighestRole = GetHighestRole(currentUserRoles);
            var targetUserHighestRole = GetHighestRole(targetUserRoles);

            if (_roleHierarchy[currentUserHighestRole] <= _roleHierarchy[targetUserHighestRole])
                return false;

            await EnsureRoleExistsAsync(roleName);
            var result = await _userManager.AddToRoleAsync(targetUser, roleName);

            return result.Succeeded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error promoting user account for {TargetUserName} by {CurrentUserName}", targetUserName, currentUserName);
            return false;
        }
    }

    private string GetHighestRole(IList<string> roles)
    {
        return roles.OrderByDescending(role => _roleHierarchy.GetValueOrDefault(role, 0)).FirstOrDefault();
    }

    private bool IsValidRole(string roleName) => _roleHierarchy.ContainsKey(roleName);

    private async Task EnsureRoleExistsAsync(string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            var role = new ARole { Name = roleName };
            await _roleManager.CreateAsync(role);
        }
    }

    private async Task<bool> UpdateUserPropertyAsync(AUser user, Action<AUser> updateAction)
    {
        try
        {
            updateAction(user);
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserName}", user.UserName);
            return false;
        }
    }

    public async Task<bool> UpdateEmailAsync(string userName, UpdateEmailRequest request)
    {
        var user = await GetUserByNameAsync(userName);
        return user != null && await UpdateUserPropertyAsync(user, u => u.Email = request.Email);
    }
    public async Task<bool> UpdateAvatarUrlAsync(string userName, UpdateAvatarUrlRequest request)
    {
        var user = await GetUserByNameAsync(userName);
        return user != null && await UpdateUserPropertyAsync(user, u => u.AvataUrl = request.AvatarUrl);
    }

    public async Task<bool> VerifyCurrentPasswordAsync(string userName, CheckPasswordRequest request)
    {
        var user = await GetUserByNameAsync(userName);
        return user != null && await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
    }

    public async Task<bool> UpdatePasswordAsync(string userName, UpdatePasswordRequest request)
    {
        try
        {
            var user = await GetUserByNameAsync(userName);
            if (user == null) return false;

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);

            return result.Succeeded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating password for {UserName}", userName);
            return false;
        }
    }

    public async Task<bool> UpdateNameAsync(string userName, UpdateNameRequest request)
    {
        var user = await GetUserByNameAsync(userName);
        return user != null && await UpdateUserPropertyAsync(user, u =>
        {
            u.FirstName = request.FirstName;
            u.LastName = request.LastName;
        });
    }

    public async Task<bool> UpdatePhoneNumberAsync(string userName, UpdatePhoneNumberRequest request)
    {
        var user = await GetUserByNameAsync(userName);
        return user != null && await UpdateUserPropertyAsync(user, u => u.PhoneNumber = request.PhoneNumber);
    }

    public async Task<bool> UpdateGenderAsync(string userName, UpdateGenderRequest request)
    {
        var user = await GetUserByNameAsync(userName);
        return user != null && await UpdateUserPropertyAsync(user, u => u.Gender = request.Gender);
    }

    public async Task<bool> UpdateDateOfBirthAsync(string userName, UpdateDateOfBornRequest request)
    {
        var user = await GetUserByNameAsync(userName);
        return user != null && await UpdateUserPropertyAsync(user, u => u.DateOfBirth = request.DateOfBirth);
    }

    public async Task<bool> GeneralUpdateAsync(string userName, GeneralUpdateRequest request)
    {
        var user = await GetUserByNameAsync(userName);
        return user != null && await UpdateUserPropertyAsync(user, u =>
        {
            if (!string.IsNullOrEmpty(request.AvatarUrl)) u.AvataUrl = request.AvatarUrl;
            if (!string.IsNullOrEmpty(request.Email)) u.Email = request.Email;
            if (!string.IsNullOrEmpty(request.FirstName)) u.FirstName = request.FirstName;
            if (!string.IsNullOrEmpty(request.LastName)) u.LastName = request.LastName;
            if (!string.IsNullOrEmpty(request.PhoneNumber)) u.PhoneNumber = request.PhoneNumber;
            if (!string.IsNullOrEmpty(request.Gender.ToString())) u.Gender = request.Gender;
            if (request.DateOfBirth != DateTime.MinValue) u.DateOfBirth = request.DateOfBirth;
        });
    }

    public async Task<bool> EnableTwoFactor(string userName)
    {
        var user = await GetUserByNameAsync(userName);
        return user != null && await UpdateUserPropertyAsync(user, u => u.TwoFactorEnabled = true);
    }

    public async Task<bool> DisableTwoFactor(string userName)
    {
        var user = await GetUserByNameAsync(userName);
        return user != null && await UpdateUserPropertyAsync(user, u => u.TwoFactorEnabled = false);
    }
}
