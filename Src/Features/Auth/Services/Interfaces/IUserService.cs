using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Src.Features.Auth.Dtos.Request;

namespace HUBT_Social_API.Features.Auth.Services.Interfaces;

public interface IUserService
{
    Task<List<string>> ConvertIdRoleToNameAsync(List<Guid> guids);
    Task<AUser?> FindUserByUserNameAsync(string userName);
    Task<AUser?> FindUserByEmailAsync(string email);
    Task<bool> PromoteUserAccountAsync(string currentUserName, string targetUserName, string roleName);

    // Phương thức cập nhật từng trường riêng lẻ
    Task<bool> UpdateAvatarUrlAsync(string userName, UpdateAvatarUrlRequest request);
    Task<bool> UpdateEmailAsync(string userName, UpdateEmailRequest request);
    Task<bool> VerifyCurrentPasswordAsync(string userName, CheckPasswordRequest request);
    Task<bool> UpdatePasswordAsync(string userName, UpdatePasswordRequest request);
    Task<bool> UpdatePhoneNumberAsync(string userName, UpdatePhoneNumberRequest request);
    Task<bool> UpdateStatusAsync(string userName, string bio);
    Task<bool> AddInfoUser(string userName, AddInfoUserRequest request);

    Task<bool> EnableTwoFactor(string userName);

    Task<bool> DisableTwoFactor(string userName);

    // Phương thức cập nhật tổng quát
    Task<bool> GeneralUpdateAsync(string userName, GeneralUpdateRequest request);

    Task<bool> DeleteUserAsync(AUser user);

    Task<bool> UpdateFcmTokenAsync(string userName, string fcmToken);

    Task<bool> StoreUserPromotionAsync(string userName, string userEmail, PromotedStoreRequest request);
}