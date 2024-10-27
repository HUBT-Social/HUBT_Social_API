using System.ComponentModel.DataAnnotations;
using HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace HUBT_Social_API.Features.Auth.Services.Child;

public class LoginService : ILoginService
{
    private readonly SignInManager<AUser> _signInManager;
    private readonly UserManager<AUser> _userManager;

    public LoginService(SignInManager<AUser> signInManager, UserManager<AUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<(SignInResult Result, AUser? User, string? ErrorMessage)> LoginAsync(ILoginRequest model)
    {
        if (model == null)
            return (SignInResult.Failed, null, "Yêu cầu đăng nhập không hợp lệ.");

        try
        {
            var (user, errorMessage) = await FindUserByIdentifierAsync(model);


            if (user == null) return (SignInResult.Failed, null, errorMessage);


            #pragma warning disable CS8604 // Possible null reference argument.
            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, false);
            #pragma warning restore CS8604 // Possible null reference argument.

            return result.Succeeded
                ? (result, user, null)
                : (SignInResult.Failed, null, "Đăng nhập không thành công.");
                
        }
        catch (Exception ex)
        {
            return (SignInResult.Failed, null, $"Đã xảy ra lỗi không xác định: {ex.Message}");
        }
    }

    private async Task<(AUser? User, string? ErrorMessage)> FindUserByIdentifierAsync(ILoginRequest model)
    {
        try
        {
            AUser? user = null;

            // Kiểm tra nếu identifier là email
            if (new EmailAddressAttribute().IsValid(model.Identifier))
            {
                user = await _userManager.FindByEmailAsync(model.Identifier);
            }
            else
            {
                user = await _userManager.FindByNameAsync(model.Identifier);
            }

            // Kiểm tra sự tồn tại của user và xác thực mật khẩu
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return (null, "Tài khoản không tồn tại hoặc mật khẩu không chính xác.");
            }

            return (user, null);
        }
        catch (Exception ex)
        {
            // Xử lý lỗi và trả về thông báo thay vì throw
            return (null, $"Lỗi khi tìm kiếm người dùng: {ex.Message}");
        }
    }
}
