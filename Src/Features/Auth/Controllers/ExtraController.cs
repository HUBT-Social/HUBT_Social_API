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
public class ExtraController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly IUserService _userService;
    private readonly IStringLocalizer<SharedResource> _localizer;

    private readonly IAuthService _authService;

    private readonly ITokenService _tokenService;

    public ExtraController(IUserService userService, IEmailService emailService, IStringLocalizer<SharedResource> localizer,IAuthService authService,ITokenService tokenService)
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
            return BadRequest(_localizer["PasswordCheckEmptyError"]);

        var result = await _userService.VerifyCurrentPasswordAsync(userResponse.Username,request);
        return result ? Ok(_localizer["PasswordCorrect"]) : BadRequest(_localizer["PasswordIncorrect"]);
    }

    // Gửi mã OTP
    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
            return BadRequest(_localizer["UsernameEmptyError"]);

        var user = await _userService.FindUserByUserNameAsync(request.Username);

        if (user == null) return BadRequest(_localizer["InvalidRequestError"]);
#pragma warning disable CS8604 // Possible null reference argument.
        var code = await _emailService.CreatePostcodeAsync(user.Email);
#pragma warning restore CS8604 // Possible null reference argument.

        var result = await _emailService.SendEmailAsync(
            new EmailRequest
            {
                Code = code.Code,
                Subject = _localizer["EmailVerificationSubject"],
                ToEmail = user.Email
            });
        return result ? Ok(_localizer["OtpSentSuccess"]) : BadRequest(_localizer["OtpSendError"]);
    }

    // Xác thực mã OTP
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] ValidatePostcodeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Postcode))
            return BadRequest(_localizer["OtpVerifyEmptyError"]);

        var result = await _emailService.ValidatePostcodeAsync(request);
        return result is not null ? Ok(_localizer["OtpVerifySuccess"]) : BadRequest(_localizer["OtpInvalid"]);
    }

    // Xác thực mã OTP và tạo token nếu thành công
    [HttpPost("confirm-code-generatetoken")]
    public async Task<IActionResult> ConfirmCode([FromBody] ValidatePostcodeRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(
                new AuthResponse(
                    false,
                    _localizer["InvalidInformation"]
                )
            );

        var user = await _authService.VerifyCodeAsync(request);
        if (user == null)
        {
            var tempUser = await _authService.GetTempUser(request.Email);
            if (tempUser == null)
                return Unauthorized(
                    new AuthResponse(
                        false,
                        _localizer["OTPVerificationFailed"]
                    ));

            var (result, registeredUser) = await _authService.RegisterAsync(new RegisterRequest
            {
                Email = tempUser.Email,
                Password = tempUser.Password,
                UserName = tempUser.UserName
            });

            if (!result.Succeeded)
                return Unauthorized(
                    new AuthResponse(
                        false,
                        _localizer["OTPVerificationFailed"]
                    ));
            user = registeredUser;
        }

        var token = await _tokenService.GenerateTokenAsync(user);

        return Ok(
            new AuthResponse(
                true,
                _localizer["VerificationSuccess"],
                new { Token = token }
            )
        );
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
            return NotFound("Không tìm thấy người dùng.");

        return Ok(user);
    }

}