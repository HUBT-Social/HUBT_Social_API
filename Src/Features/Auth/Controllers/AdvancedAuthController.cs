using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Controllers;
using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBT_Social_API.Src.Core.Helpers;
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
                    AvatarUrl = userResponse.User.AvataUrl,
                    FirstName = userResponse.User.FirstName,
                    LastName = userResponse.User.LastName,
                    Gender = userResponse.User.Gender,
                    Email = userResponse.User.Email,
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
    [HttpPost("forgot-password/get-user")]
    public async Task<IActionResult> SearchByUserNameOrEmail([FromBody] SearchUserByUserNameOrPasswordRequest request)
    {
        // Kiểm tra đầu vào có phải là email không
        bool isEmail = request.UserNameOrEmail.Contains("@gmail.com");

        AUser? user;

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
                        AvatarUrl = user.AvataUrl,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Gender = user.Gender,
                        Email = user.Email,
                        BirthDay = user.DateOfBirth,
                        PhoneNumber = user.PhoneNumber
                    });
        }

        return NotFound(LocalValue.Get(KeyStore.UserNotFound));

        
    }

    
    [HttpGet("get-mask-email")]
    public async Task<IActionResult> GetCurrentEmail()
    {
        // 1. Lấy UserAgent từ header
        string userAgent = Request.Headers["User-Agent"].ToString();
        if (string.IsNullOrWhiteSpace(userAgent))
            return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));

        // 2. Lấy địa chỉ IP
        string? ipAddress = ServerHelper.GetIPAddress(HttpContext);
        if (string.IsNullOrWhiteSpace(ipAddress))
            return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));

        // 3. Lấy email từ dịch vụ
        string? currentEmail = await _emailService.GetValidateEmail(userAgent, ipAddress);
        if (string.IsNullOrWhiteSpace(currentEmail))
            return BadRequest(LocalValue.Get(KeyStore.InvalidCredentials));

        // 4. Thực hiện mask email
        if (_emailService.MaskEmail(currentEmail, out string maskedEmail))
            return Ok(new { MaskedEmail = maskedEmail });

        // 5. Xử lý lỗi nếu không thể mask email
        return BadRequest(LocalValue.Get(KeyStore.InvalidCredentials));
    }

    [HttpGet("forgot-password/get-mask-email")]
    public async Task<IActionResult> GetMaskPassword(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest(LocalValue.Get(KeyStore.EmailCannotBeEmpty));

        // 4. Thực hiện mask email
        if (_emailService.MaskEmail(email, out string maskedEmail))
            return Ok(new { MaskedEmail = maskedEmail });

        // 5. Xử lý lỗi nếu không thể mask email
        return BadRequest(LocalValue.Get(KeyStore.InvalidCredentials));
    }
    // Gửi mã OTP
    [HttpPost("forgot-password/send-otp")]
    public async Task<IActionResult> SendOtp(string email)
    {
        try
        {
            // 1. Kiểm tra input
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(LocalValue.Get(KeyStore.EmailCannotBeEmpty));

            if (!_emailService.IsValidEmail(email))
                return BadRequest(LocalValue.Get(KeyStore.InvalidRequestError));

            // 2. Lấy UserAgent và IP Address
            string userAgent = Request.Headers.UserAgent.ToString();
            string? ipAddress = ServerHelper.GetIPAddress(HttpContext);
            if (ipAddress == null)
                return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));

            // 3. Tạo mã OTP
            Postcode? code = await _emailService.CreatePostcodeAsync(userAgent, email, ipAddress);
            if (code == null)
                return BadRequest(LocalValue.Get(KeyStore.InvalidCredentials));

            // 4. Gửi email
            var result = await _emailService.SendEmailAsync(new EmailRequest
            {
                Code = code.Code,
                Subject = LocalValue.Get(KeyStore.EmailVerificationCodeSubject),
                ToEmail = email
            });

            return result ? Ok(LocalValue.Get(KeyStore.OtpSent)) : BadRequest(LocalValue.Get(KeyStore.OtpSendError));

        }
        catch (Exception ex)
        {
           return BadRequest(LocalValue.Get(KeyStore.OtpSendError));
        }
    }

    // Xác thực mã OTP
    [HttpPost("forgot-password/password-verify")]
    public async Task<IActionResult> VerifyOtp([FromBody] OTPRequest request)
    {
        string userAgent = Request.Headers.UserAgent.ToString();
        string? ipAddress = ServerHelper.GetIPAddress(HttpContext);
        if (ipAddress == null) return BadRequest(
            new LoginResponse
            {
                RequiresTwoFactor = false,
                Message = LocalValue.Get(KeyStore.InvalidInformation)
            }
            );

        string? currentEmail = await _emailService.GetValidateEmail(userAgent,ipAddress);

        // 3. Kiểm tra nếu email không tồn tại
        if (currentEmail == null)
            return BadRequest(LocalValue.Get(KeyStore.UserNotFound));

        // 4. Xác thực mã OTP
        var result = await _emailService.ValidatePostcodeAsync(new ValidatePostcodeRequest
        {
            Postcode = request.Postcode,
            Email = currentEmail,
            UserAgent = userAgent
        });

        // 5. Trả về kết quả
        return result is not null
            ? Ok(LocalValue.Get(KeyStore.OtpVerificationSuccess))
            : BadRequest(LocalValue.Get(KeyStore.OtpInvalid));
    }
    [HttpPost("forgot-password/change-password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
    {
        if(request.NewPassword == request.ConfirmNewPassword)
        {
            await UpdateHelper.HandleUserUpdate(KeyStore.PasswordUpdated, KeyStore.PasswordUpdateError, _userService.UpdatePasswordAsync, request,Request,_tokenService);
        }
        return BadRequest(LocalValue.Get(KeyStore.ConfirmPasswordError));
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