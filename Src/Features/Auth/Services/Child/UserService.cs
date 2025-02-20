using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.Src.Core.Helpers;
using HUBT_Social_API.Src.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Src.Features.Auth.Dtos.Request;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace HUBT_Social_API.Features.Auth.Services.Child;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IMongoCollection<PromotedStore> _promotedStore;

    private readonly Dictionary<string, int> _roleHierarchy = new()
    {
        { "ADMIN", 4 },
        { "HEAD", 3 },
        { "TEACHER", 2 },
        { "STUDENT", 1 },
        { "USER", 0 }
    };

    private readonly RoleManager<ARole> _roleManager;
    private readonly UserManager<AUser> _userManager;

    public UserService(UserManager<AUser> userManager, RoleManager<ARole> roleManager, ILogger<UserService> logger,
        IMongoCollection<PromotedStore> prometedStore = null)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
        _promotedStore = prometedStore;
    }
    public async Task<List<string>> ConvertIdRoleToNameAsync(List<Guid> guids)
    {
        return await TokenHelper.ConvertRolesIdtoRolesName(guids,_roleManager);
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
            _logger.LogError(ex, "Error promoting user account for {TargetUserName} by {CurrentUserName}",
                targetUserName, currentUserName);
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
    public async Task<bool> UpdatePhoneNumberAsync(string userName, UpdatePhoneNumberRequest request)
    {

        var user = await GetUserByNameAsync(userName);
        return user != null && await UpdateUserPropertyAsync(user, u => u.Email = request.PhoneNumber);
    }


    public async Task<bool> GeneralUpdateAsync(string userName, GeneralUpdateRequest request)
    {
        var user = await GetUserByNameAsync(userName);
        return user != null && await UpdateUserPropertyAsync(user, u =>
        {
            if (!string.IsNullOrEmpty(request.FirstName)) u.FirstName = request.FirstName;
            if (!string.IsNullOrEmpty(request.LastName)) u.LastName = request.LastName;
            if (!string.IsNullOrEmpty(request.Gender.ToString())) u.Gender = request.Gender;
            u.DateOfBirth = request.DateOfBirth.ToString() != null ? request.DateOfBirth : DateTime.MinValue;
        });
    }

    public async Task<bool> AddInfoUser(string userName, AddInfoUserRequest request)
    {
        var user = await GetUserByNameAsync(userName);
        return user != null && await UpdateUserPropertyAsync(user, u =>
        {
            u.FirstName = request.FirstName;
            u.LastName = request.LastName;
            u.PhoneNumber = request.PhoneNumber;
            u.Gender = request.Gender;
            u.DateOfBirth = request.DateOfBirth;
        });
    }

    public async Task<bool> DeleteUserAsync(AUser user)
    {
        var deleted = await _userManager.DeleteAsync(user);
        return deleted.Succeeded;
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

    public async Task<bool> UpdateFcmTokenAsync(string userName, string fcmToken)
    {
        try
        {
            var user = await GetUserByNameAsync(userName);
            return user != null && await UpdateUserPropertyAsync(user, u => u.FCMToken = fcmToken);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> UpdateStatusAsync(string userName, string bio)
    {
        var user = await GetUserByNameAsync(userName);
        return user != null && await UpdateUserPropertyAsync(user, u => u.status = bio);
    }

    public async Task<bool> StoreUserPromotionAsync(string userId, string userEmail, PromotedStoreRequest request)
    {
        try
        {
            var fcmToken = await _promotedStore.Find(
                    fcm => fcm.UserId == userId)
                .FirstOrDefaultAsync();
            if (fcmToken == null)
            {
                PromotedStore newFcmToken = new()
                {
                    IdentifyCode = request.IdentifyCode,
                    UserId = userId,
                    email = userEmail,
                    ExpireTime = DateTime.UtcNow
                };
                await _promotedStore.InsertOneAsync(newFcmToken);
            }
            else
            {
                var update = Builders<PromotedStore>.Update
                    .Set(t => t.email, userEmail)
                    .Set(t => t.IdentifyCode, request.IdentifyCode)
                    .Set(t => t.ExpireTime, DateTime.UtcNow);
                await _promotedStore.UpdateOneAsync(
                    fcm => fcm.UserId == userId
                    , update);
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private async Task<AUser?> GetUserByNameAsync(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName)) return null;
        return await _userManager.FindByNameAsync(userName);
    }

    private string GetHighestRole(IList<string> roles)
    {
        return roles.OrderByDescending(role => _roleHierarchy.GetValueOrDefault(role, 0)).FirstOrDefault();
    }

    private bool IsValidRole(string roleName)
    {
        return _roleHierarchy.ContainsKey(roleName);
    }

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
}
