using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Controllers;
using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HUBT_Social_API.Controllers;

[ApiController]
[Route("api/auth")]
public class AdvancedAuthController : BaseAuthController
{
    private readonly IImageService _imageService;
    public AdvancedAuthController(IAuthService authService, ITokenService tokenService, IEmailService emailService,IUserService userService,IImageService imageService)
    :base (authService,tokenService,emailService,null,userService)
    {
        _imageService = imageService;
    }


    [HttpGet("get-user")]
    public async Task<IActionResult> GetCurrentUser()
    {
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (userResponse.Success && userResponse.User != null)
        {
            return Ok(
                new {
                    IdUser = userResponse.User.Id,
                    Email = userResponse.User.Email,
                    FirstName = userResponse.User.FirstName,
                    LastName = userResponse.User.LastName,
                    Gender = userResponse.User.Gender,
                    AvatarUrl = userResponse.User.AvataUrl,
                    BirthDay = userResponse.User.DateOfBirth,
                    PhoneNumber = userResponse.User.PhoneNumber
                }
            );
        }
        
        return BadRequest(userResponse.Message);

        
    }

    // Kiểm tra mật khẩu hiện tại
    [HttpPost("check-password")]
    public async Task<IActionResult> CheckPassword([FromBody] CheckPasswordRequest request)
    {
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);

        if (request.CurrentPassword == null || userResponse.User == null || userResponse.User.UserName == null)
            
            return BadRequest(LocalValue.Get(KeyStore.PasswordCheckEmptyError));

        var result = await _userService.VerifyCurrentPasswordAsync(userResponse.User.UserName,request);
        return result ? Ok(LocalValue.Get(KeyStore.PasswordCorrect)) : BadRequest(LocalValue.Get(KeyStore.PasswordIncorrect));
    }

    
    //Tim tai khoan de dang nhap, bang username or password
    [HttpPost("get-user-by-usename-or-email")]
    public async Task<IActionResult> SearchByUserNameOrEmail([FromBody] SearchUserByUserNameOrPasswordRequest request)
    {
        // Kiểm tra đầu vào có phải là email không
        bool isEmail = request.UserNameOrEmail.Contains("@");

        Features.Auth.Models.AUser? user;

        if (isEmail)
        {
            // Tìm người dùng theo email
            user = await _userService.FindUserByEmailAsync(request.UserNameOrEmail);
        }
        else
        {
            // Tìm người dùng theo tên đăng nhập
            user = await _userService.FindUserByUserNameAsync(request.UserNameOrEmail);
        }

        if (user != null)
        {
            return Ok(
                new {
                        IdUser = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Gender = user.Gender,
                        AvatarUrl = user.AvataUrl,
                        BirthDay = user.DateOfBirth,
                        PhoneNumber = user.PhoneNumber
                    });
        }

        return NotFound(LocalValue.Get(KeyStore.UserNotFound));

        
    }

    
    [HttpGet("get-mask-email")]
    public async Task<IActionResult> GetCurrentEmail()
    {
        string userAgent = Request.Headers.UserAgent.ToString();
        string? ipAddress = TokenHelper.GetIPAddress(HttpContext);
        if (ipAddress == null) return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));

        string? currentEmail = await _emailService.GetValidateEmail(userAgent,ipAddress.ToString());
        if (string.IsNullOrEmpty(currentEmail))
            return BadRequest(LocalValue.Get(KeyStore.InvalidCredentials));
        
        // Thực hiện mask email, nếu thất bại trả về lỗi
        if (_emailService.MaskEmail(currentEmail, out string maskedEmail))
            return Ok(maskedEmail);

        return BadRequest(LocalValue.Get(KeyStore.InvalidCredentials));
        
        
    }
    [HttpPost("get-url-from-image")]
    public async Task<IActionResult> GetUrlFromImage(IFormFile file)
    {
        string avatarUrl;
        try
        {
            avatarUrl = await _imageService.GetUrlFormFileAsync(file); 
        }
        catch(Exception ex)
        {
            return BadRequest(
                new {
                    Success = false,
                    Data = $"{LocalValue.Get(KeyStore.InvalidFileData)}"
                });
        }
        if(avatarUrl != null)
        {
            return Ok(
                new {
                    Success = true,
                    Data = avatarUrl
                });
        }
        return BadRequest(
            new {
                Success = false,
                Data = $"{LocalValue.Get(KeyStore.InvalidFileData)}"
            }
        );
    }

}