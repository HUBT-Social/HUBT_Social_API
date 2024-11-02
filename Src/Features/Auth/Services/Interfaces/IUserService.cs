using HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;
using HUBT_Social_API.Features.Auth.Models;

namespace HUBT_Social_API.Features.Auth.Services.Interfaces;

public interface IUserService
{
    Task<AUser?> FindUserByUserNameAsync(string userName);
    Task<bool> PromoteUserAccountAsync(string userName, string roleName);

    // Phương thức cập nhật từng trường riêng lẻ
    Task<bool> UpdateEmailAsync(string userName, UpdateEmailRequest request);
    Task<bool> VerifyCurrentPasswordAsync(string userName, CheckPasswordRequest request);
    Task<bool> UpdatePasswordAsync(string userName, UpdatePasswordRequest request);
    Task<bool> UpdateNameAsync(string userName, UpdateNameRequest request);
    Task<bool> UpdatePhoneNumberAsync(string userName, UpdatePhoneNumberRequest request);
    Task<bool> UpdateGenderAsync(string userName, UpdateGenderRequest request);
    Task<bool> UpdateDateOfBirthAsync(string userName, UpdateDateOfBornRequest request);

    // Phương thức cập nhật tổng quát
    Task<bool> GeneralUpdateAsync(string userName, GeneralUpdateRequest request);
}