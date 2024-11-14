using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HUBT_Social_API.Controllers;

[ApiController]
[Route("api/auth")]
public partial class AccountController : ControllerBase
{
    private readonly IEmailService _emailService;   
    private readonly IUserService _userService;
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly ITokenService _tokenService;
    private readonly IAuthService _authService;

    

    public AccountController(IUserService userService, IEmailService emailService, IStringLocalizer<SharedResource> localizer,IAuthService authService,ITokenService tokenService)
    {
        _userService = userService;
        _emailService = emailService;
        _localizer = localizer;
        _authService = authService;
        _tokenService = tokenService;
    }

    // Kiểm tra mật khẩu hiện tại
    [HttpPost("check-password")]
    public async Task<IActionResult> CheckPassword([FromBody] CheckPasswordRequest request)
    {
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);

        if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(userResponse.Username))
            return BadRequest(LocalValue.Get(KeyStore.PasswordCheckEmptyError));

        var result = await _userService.VerifyCurrentPasswordAsync(userResponse.Username,request);
        return result ? Ok(LocalValue.Get(KeyStore.PasswordCorrect)) : BadRequest(LocalValue.Get(KeyStore.PasswordIncorrect));
    }

    // Gửi mã OTP
    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp()
    {
        string userAgent = Request.Headers["User-Agent"].ToString();
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);


        if (userResponse == null || userResponse.Email == null) return BadRequest(LocalValue.Get(KeyStore.InvalidRequestError));


        Postcode? code = await _emailService.CreatePostcodeAsync(userAgent,userResponse.Email);
        if (code == null) return BadRequest(              
                   LocalValue.Get(KeyStore.InvalidCredentials)
            );
        var result = await _emailService.SendEmailAsync(
            new EmailRequest
            {
                Code = code.Code,
                Subject = LocalValue.Get(KeyStore.EmailVerificationCodeSubject),
                ToEmail = userResponse.Email
            });
        return result ? Ok(LocalValue.Get(KeyStore.OtpSent)) : BadRequest(LocalValue.Get(KeyStore.OtpSendError));
    }

    // Xác thực mã OTP
    [HttpPost("verify-two-factor")]
    public async Task<IActionResult> VerifyOtp([FromBody] OTPRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Postcode))
            return BadRequest(LocalValue.Get(KeyStore.OtpVerifyEmptyError));
        string userAgent = Request.Headers["User-Agent"].ToString();

        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);


        if (userResponse.Email == null) return BadRequest(LocalValue.Get(KeyStore.UserNotFound));
        

        var result = await _emailService.ValidatePostcodeAsync(new ValidatePostcodeRequest
        {
            Postcode = request.Postcode,
            Email = userResponse.Email,
            UserAgent = userAgent
        });

        return result is not null ? Ok(LocalValue.Get(KeyStore.OtpVerificationSuccess)) : BadRequest(LocalValue.Get(KeyStore.OtpInvalid));
    }
    //Tim tai khoan de dang nhap, bang username or password
    [HttpPost("search-user-by-usename-or-email")]
    public async Task<IActionResult> SearchByUserNameOrEmail([FromBody] SearchUserByUserNameOrPasswordRequest request)
    {
        // Kiểm tra đầu vào có phải là email không
        bool isEmail = request.UserNameOrEmail.Contains("@");

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

        if (user == null)
            return NotFound(LocalValue.Get(KeyStore.UserNotFound));

        return Ok(user);
    }

    [HttpGet("token-is-validate")]
    public IActionResult ValidateToken()
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        DecodeTokenResponse result = _tokenService.ValidateToken(token);
        if (result.Success)
        {
            return Ok(_localizer["TokenValid"].Value);
        }

        return BadRequest(result.Message);
    }

}