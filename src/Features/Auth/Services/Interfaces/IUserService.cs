using HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;
using HUBT_Social_API.Features.Auth.Models;

namespace HUBT_Social_API.Features.Auth.Services.Interfaces;

public interface IUserService
{
    Task<AUser?> FindUserByUserNameAsync(string userName);
    Task<bool> PromoteUserAccountAsync(string userName, string roleName);

    // Phương thức cập nhật từng trường riêng lẻ
    Task<bool> UpdateEmailAsync(UpdateEmailRequest request);
    Task<bool> VerifyCurrentPasswordAsync(CheckPasswordRequest request);
    Task<bool> UpdatePasswordAsync(UpdatePasswordRequest request);
    Task<bool> UpdateNameAsync(UpdateNameRequest request);
    Task<bool> UpdatePhoneNumberAsync(UpdatePhoneNumberRequest request);
    Task<bool> UpdateGenderAsync(UpdateGenderRequest request);
    Task<bool> UpdateDateOfBirthAsync(UpdateDateOfBornRequest request);

    // Phương thức cập nhật tổng quát
    Task<bool> GeneralUpdateAsync(GeneralUpdateRequest request);
}