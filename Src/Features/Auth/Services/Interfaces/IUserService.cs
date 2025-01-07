using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;
using HUBT_Social_API.Features.Auth.Models;

namespace HUBT_Social_API.Features.Auth.Services.Interfaces;

public interface IUserService
{
    Task<string> GetFullName(string userName);
    Task<AUser?> FindUserByUserNameAsync(string userName);
    Task<AUser?> FindUserByEmailAsync(string email);
    Task<bool> PromoteUserAccountAsync(string currentUserName, string targetUserName, string roleName);

    // Phương thức cập nhật từng trường riêng lẻ
    Task<bool> UpdateAvatarUrlAsync(string userName, UpdateAvatarUrlRequest request);
    Task<bool> UpdateEmailAsync(string userName, UpdateEmailRequest request);
    Task<bool> VerifyCurrentPasswordAsync(string userName, CheckPasswordRequest request);
    Task<bool> UpdatePasswordAsync(string userName, UpdatePasswordRequest request);
    Task<bool> AddInfoUser(string userName, AddInfoUserRequest request);

    Task<bool> EnableTwoFactor(string userName);

    Task<bool> DisableTwoFactor(string userName);

    // Phương thức cập nhật tổng quát
    Task<bool> GeneralUpdateAsync(string userName, GeneralUpdateRequest request);

    Task<bool> DeleteUserAsync(AUser user);
}