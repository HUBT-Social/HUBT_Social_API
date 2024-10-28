using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.src.Features.Auth.Dtos.Collections;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace HUBT_Social_API.Features.Auth.Services.Child;

public class RegisterService : IRegisterService
{
    private readonly RoleManager<ARole> _roleManager;
    private readonly UserManager<AUser> _userManager;
    private readonly IMongoCollection<TempUserRegister> _tempUserRegister;

    public RegisterService(UserManager<AUser> userManager, RoleManager<ARole> roleManager, IMongoCollection<TempUserRegister> tempUserRegister)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tempUserRegister = tempUserRegister;
    }

    public async Task<bool> AddToTempUser(RegisterRequest model)
    {
        if (model == null) return false;

        TempUserRegister tempUser = new()
        {
            Email = model.Email,
            ExpireTime = DateTime.UtcNow,
            UserName = model.UserName,
            Password = model.Password
        };
        await _tempUserRegister.InsertOneAsync(tempUser);
        return true;
    }

    public async Task<bool> CheckUserAccount(RegisterRequest model)
    {
        return await _userManager.FindByNameAsync(model.UserName) != null ||
               await _userManager.FindByEmailAsync(model.Email) != null;
    }

    
    public async Task<(IdentityResult Result, string? Error)> RegisterAsync(RegisterRequest model)
    {
        if (model == null)
            return (IdentityResult.Failed(new IdentityError { Description = "Model không thể null." }), "Model không thể null.");

        try
        {
            // Kiểm tra tài khoản đã tồn tại
            
            if (await CheckUserAccount(model))
                return (IdentityResult.Failed(new IdentityError { Description = "Tài khoản đã được đăng ký." }), "Tài khoản đã tồn tại.");
            
            // Tạo người dùng mới
            var user = new AUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            // Tạo tài khoản và kiểm tra kết quả
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return (result, $"Không thể tạo tài khoản. Lỗi: {errors}");
            }

            // Xác định vai trò mặc định
            const string defaultRole = "USER";

            // Kiểm tra và tạo vai trò nếu chưa có
            if (!await _roleManager.RoleExistsAsync(defaultRole))
            {
                var roleResult = await _roleManager.CreateAsync(new ARole { Name = defaultRole });
                if (!roleResult.Succeeded)
                {
                    var roleErrors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                    return (IdentityResult.Failed(), $"Không thể tạo vai trò mặc định. Lỗi: {roleErrors}");
                }
            }

            // Thêm Claims cho người dùng
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Role, defaultRole),
                new(ClaimTypes.Email, user.Email)
            };

            // Gán vai trò và Claims cho người dùng
            var roleAssignmentResult = await _userManager.AddToRoleAsync(user, defaultRole);
            if (!roleAssignmentResult.Succeeded)
            {
                var roleAssignErrors = string.Join("; ", roleAssignmentResult.Errors.Select(e => e.Description));
                return (IdentityResult.Failed(), $"Không thể gán vai trò cho người dùng. Lỗi: {roleAssignErrors}");
            }

            var claimResult = await _userManager.AddClaimsAsync(user, claims);
            if (!claimResult.Succeeded)
            {
                var claimErrors = string.Join("; ", claimResult.Errors.Select(e => e.Description));
                return (IdentityResult.Failed(), $"Không thể gán claims cho người dùng. Lỗi: {claimErrors}");
            }

            return (IdentityResult.Success, null);
        }
        catch (Exception ex)
        {
            // Bắt các lỗi bất ngờ và trả về thông báo lỗi
            return (IdentityResult.Failed(new IdentityError { Description = ex.Message }), $"Đã xảy ra lỗi không xác định: {ex.Message}");
        }
    }


}