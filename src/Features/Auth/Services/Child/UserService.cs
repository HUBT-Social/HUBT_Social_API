using HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;
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

    public async Task<AUser?> FindUserByUserNameAsync(string userName)
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

    public async Task<bool> UpdateEmailAsync(UpdateEmailRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);
        if (user == null) return false;

        user.Email = request.Email;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> VerifyCurrentPasswordAsync(CheckPasswordRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null) return false;

        // Kiểm tra mật khẩu hiện tại
        var passwordCheck = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
        return passwordCheck; // Trả về true nếu mật khẩu đúng, ngược lại là false
    }

    public async Task<bool> UpdatePasswordAsync(UpdatePasswordRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);
        if (user == null) return false;

        // Tạo token đặt lại mật khẩu
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        // Đặt lại mật khẩu với token và mật khẩu mới
        var result = await _userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);
        return result.Succeeded;
    }

    public async Task<bool> UpdateNameAsync(UpdateNameRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);
        if (user == null) return false;

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> UpdatePhoneNumberAsync(UpdatePhoneNumberRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null) return false;

        user.PhoneNumber = request.PhoneNumber;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> UpdateGenderAsync(UpdateGenderRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null) return false;

        user.IsMale = request.IsMale;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> UpdateDateOfBirthAsync(UpdateDateOfBornRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null) return false;

        user.DateOfBirth = request.DateOfBirth;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> GeneralUpdateAsync(GeneralUpdateRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
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


    // Tương tự cho các phương thức PromoteToTeacherAsync, VerifyCodeAsync, v.v.
}