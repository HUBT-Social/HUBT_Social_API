using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
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
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        UserResponse userResponse = await _tokenService.GetCurrentUser(token);

        if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(userResponse.Username))
            return BadRequest(_localizer["PasswordCheckEmptyError"].Value);

        var result = await _userService.VerifyCurrentPasswordAsync(userResponse.Username,request);
        return result ? Ok(_localizer["PasswordCorrect"].Value) : BadRequest(_localizer["PasswordIncorrect"].Value);
    }

    // Gửi mã OTP
    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp()
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        UserResponse userResponse = await _tokenService.GetCurrentUser(token);


        if (userResponse == null || userResponse.Email == null) return BadRequest(_localizer["InvalidRequestError"].Value);


        Postcode? code = await _emailService.CreatePostcodeAsync(userResponse.Email);
        if (code == null) return BadRequest(
                new
                {
                    message = _localizer["InvalidCredentials"].Value
                }
            );
        var result = await _emailService.SendEmailAsync(
            new EmailRequest
            {
                Code = code.Code,
                Subject = _localizer["EmailVerificationCodeSubject"].Value,
                ToEmail = userResponse.Email
            });
        return result ? Ok(_localizer["OtpSent"].Value) : BadRequest(_localizer["OtpSendError"].Value);
    }

    // Xác thực mã OTP
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] OTPRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Postcode))
            return BadRequest(_localizer["OtpVerifyEmptyError"].Value);

        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        UserResponse? userResponse = await _tokenService.GetCurrentUser(token);


        if (userResponse.Email == null) return BadRequest(_localizer["UserNotFound"].Value);
        

        var result = await _emailService.ValidatePostcodeAsync(new ValidatePostcodeRequest
        {
            Postcode = request.Postcode,
            Email = userResponse.Email,
        });

        return result is not null ? Ok(_localizer["OtpVerificationSuccess"].Value) : BadRequest(_localizer["OtpInvalid"].Value);
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
            return NotFound(_localizer["UserNotFound"].Value);

        return Ok(user);
    }

}