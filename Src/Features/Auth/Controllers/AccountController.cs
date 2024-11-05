using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HUBT_Social_API.Features.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly IRegisterService _registerService;
    private readonly ITokenService _tokenService;

    public AccountController(IAuthService authService, IStringLocalizer<SharedResource> localizer,
        ITokenService tokenService, IEmailService emailService, IRegisterService registerService)
    {
        _authService = authService;
        _localizer = localizer;
        _tokenService = tokenService;
        _emailService = emailService;
        _registerService = registerService;
    }

    // Đăng ký tài khoản mới và gửi mã OTP qua email
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(
                new AuthResponse(
                    false,
                    _localizer["InvalidInformation"]
                )
            );
        if (await _registerService.CheckUserAccountExit(request))
            return BadRequest(
                new AuthResponse(
                    false,
                    _localizer["UserAlreadyExists"]
                )
            );
        if (!await _registerService.AddToTempUser(request))
            return BadRequest(
                new AuthResponse(
                    false,
                    _localizer["UnableToStoreInDatabase"]
                )
            );

        // Gửi mã OTP qua email để xác thực
        try
        {
            var code = await _emailService.CreatePostcodeAsync(request.Email);

            await _emailService.SendEmailAsync(new EmailRequest
                { Code = code.Code, Subject = _localizer["EmailVerificationCodeSubject"], ToEmail = request.Email });
        }
        catch (Exception)
        {
            return StatusCode(
                500,
                new AuthResponse(
                    false,
                    _localizer["UnableToSendOTP"]
                )
            );
        }

        return Ok(
            new AuthResponse(
                true,
                _localizer["RegistrationSuccess"]
            )
        );
    }

    // Đăng nhập và gửi mã OTP qua email
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginByStudentCodeRequest model)
    {
        var (result, user) = await _authService.LoginAsync(model);

        if (result.Succeeded && user is not null)
        {
            try
            {
                var code = await _emailService.CreatePostcodeAsync(user.Email);

                await _emailService.SendEmailAsync(
                    new EmailRequest
                    {
                        Code = code.Code,
                        Subject = _localizer["EmailVerificationCodeSubject"],
                        ToEmail = user.Email
                    });
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    new AuthResponse(
                        false,
                        _localizer["UnableToSendOTP"]
                    )
                );
            }

            return Ok(
                new AuthResponse(
                    true,
                    _localizer["StepOneVerificationSuccess"]
                )
            );
        }

        if (result.IsLockedOut)
            return BadRequest(
                new AuthResponse(
                    false,
                    _localizer["AccountLocked"]
                )
            );
        if (result.IsNotAllowed)
            return BadRequest(
                new AuthResponse(
                    false,
                    _localizer["LoginNotAllowed"]
                )
            );
        if (result.RequiresTwoFactor)
            return BadRequest(
                new AuthResponse(
                    false,
                    _localizer["TwoFactorRequired"]
                )
            );
        return BadRequest(
            new AuthResponse(
                false,
                _localizer["InvalidCredentials"]
            )
        );
    }

    // Xác thực mã OTP và tạo token nếu thành công
    [HttpPost("confirm-code")]
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
}
