using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.src.Features.Auth.Dtos.Collections;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Features.Auth.Controllers;

public class OTPController : BaseAccountController
{
    public OTPController(IAuthService authService, ITokenService tokenService, IEmailService emailService)
    :base (authService,tokenService,emailService,null,null)
    {

    }
    // Gửi mã OTP
    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp()
    {
        string userAgent = Request.Headers.UserAgent.ToString();
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);


        if (userResponse == null || userResponse.User == null|| userResponse.User.Email == null) return BadRequest(LocalValue.Get(KeyStore.InvalidRequestError));


        Postcode? code = await _emailService.CreatePostcodeAsync(userAgent,userResponse.User.Email);
        if (code == null) return BadRequest(              
                   LocalValue.Get(KeyStore.InvalidCredentials)
            );
        var result = await _emailService.SendEmailAsync(
            new EmailRequest
            {
                Code = code.Code,
                Subject = LocalValue.Get(KeyStore.EmailVerificationCodeSubject),
                ToEmail = userResponse.User.Email
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


        if (userResponse == null || userResponse.User == null|| userResponse.User.Email == null) return BadRequest(LocalValue.Get(KeyStore.UserNotFound));
        

        var result = await _emailService.ValidatePostcodeAsync(new ValidatePostcodeRequest
        {
            Postcode = request.Postcode,
            Email = userResponse.User.Email,
            UserAgent = userAgent
        });

        return result is not null ? Ok(LocalValue.Get(KeyStore.OtpVerificationSuccess)) : BadRequest(LocalValue.Get(KeyStore.OtpInvalid));
    }
    [HttpPost("sign-up/verify-otp")]
    public async Task<IActionResult> ConfirmCodeSignUp([FromBody] OTPRequest code)
    {
        string userAgent = Request.Headers.UserAgent.ToString();

        string? currentEmail = await _emailService.GetValidateEmail(userAgent);
        if (currentEmail == null) return BadRequest(
            new LoginResponse
            {
                RequiresTwoFactor = false,
                Message = LocalValue.Get(KeyStore.InvalidInformation)
            }
            );

        ValidatePostcodeRequest request = new()
        {
            Postcode = code.Postcode,
            UserAgent = userAgent,
            Email = currentEmail
        };


        if (!ModelState.IsValid)
            return BadRequest(
                new LoginResponse
                {
                    RequiresTwoFactor = false,
                    Message = LocalValue.Get(KeyStore.InvalidInformation)
                }
            );

        
        TempUserRegister? tempUser = await _authService.GetTempUser(request.Email);
        if (tempUser == null)
            return Unauthorized(
                 new LoginResponse
                 {
                     RequiresTwoFactor = false,
                     Message = LocalValue.Get(KeyStore.OTPVerificationFailed)
                 }
                );

        var (result, registeredUser) = await _authService.RegisterAsync(new RegisterRequest
        {
            Email = tempUser.Email,
            Password = tempUser.Password,
            UserName = tempUser.UserName
        });

        if (!result.Succeeded || registeredUser is null)
            return Unauthorized(new LoginResponse
            {
                RequiresTwoFactor = false,
                Message = LocalValue.Get(KeyStore.OTPVerificationFailed)
            });

        AUser? user = await _authService.VerifyCodeAsync(request);
        if (user == null)
        {
            return Unauthorized(new LoginResponse
            {
                RequiresTwoFactor = false,
                Message = LocalValue.Get(KeyStore.OTPVerificationFailed)
            }
            );
        }

        TokenResponse token = await _tokenService.GenerateTokenAsync(user);

        return Ok(
            new LoginResponse
            {
                RequiresTwoFactor = false,
                Message = LocalValue.Get(KeyStore.VerificationSuccess),
                UserToken = token
            }
        );
    }

    [HttpPost("sign-in/verify-two-factor")]
    public async Task<IActionResult> ConfirmCodeSignIn([FromBody] OTPRequest code)
    {
        string userAgent = Request.Headers.UserAgent.ToString();

        string? currentEmail = await _emailService.GetValidateEmail(userAgent);
        if (currentEmail == null) return BadRequest(
            new LoginResponse
            {
                RequiresTwoFactor = false,
                Message = LocalValue.Get(KeyStore.InvalidInformation)
            }
            );

        ValidatePostcodeRequest request = new()
        {
            Postcode = code.Postcode,
            UserAgent = userAgent,
            Email = currentEmail
        };

        if (!ModelState.IsValid)
            return BadRequest(
                new LoginResponse
                {
                    RequiresTwoFactor = false,
                    Message = LocalValue.Get(KeyStore.InvalidInformation)
                }
                
            );

        var user = await _authService.VerifyCodeAsync(request);
        if (user == null)
        {
            return Unauthorized(
                new LoginResponse
                {
                    RequiresTwoFactor = false,
                    Message = LocalValue.Get(KeyStore.OTPVerificationFailed)
                }
                        
            );
        }

        TokenResponse token = await _tokenService.GenerateTokenAsync(user);

        return Ok(
            new LoginResponse
            {
                RequiresTwoFactor = false,
                Message = LocalValue.Get(KeyStore.VerificationSuccess),
                UserToken = token
            }
        );
    }

}