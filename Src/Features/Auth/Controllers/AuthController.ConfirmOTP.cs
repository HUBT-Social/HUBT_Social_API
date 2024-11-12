using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.src.Features.Auth.Dtos.Collections;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Features.Auth.Controllers;

public partial class AccountController
{
    [HttpPost("sign-up/auth2")]
    public async Task<IActionResult> ConfirmCodeSignUp([FromBody] OTPRequest code)
    {
        string userAgent = Request.Headers["User-Agent"].ToString();

        string? currentEmail = await _emailService.GetValidateEmail(userAgent);
        if (currentEmail == null) return BadRequest(
            new LoginResponse
            {
                RequiresTwoFactor = false,
                Message = _localizer["InvalidInformation"].Value
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
                    Message = _localizer["InvalidInformation"].Value
                }
            );

        
        TempUserRegister? tempUser = await _authService.GetTempUser(request.Email);
        if (tempUser == null)
            return Unauthorized(
                 new LoginResponse
                 {
                     RequiresTwoFactor = false,
                     Message = _localizer["OTPVerificationFailed"].Value
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
                Message = _localizer["OTPVerificationFailed"].Value
            });

        AUser? user = await _authService.VerifyCodeAsync(request);
        if (user == null)
        {
            return Unauthorized(new LoginResponse
            {
                RequiresTwoFactor = false,
                Message = _localizer["OTPVerificationFailed"].Value
            }
            );
        }

        TokenResponse token = await _tokenService.GenerateTokenAsync(user);

        return Ok(
            new LoginResponse
            {
                RequiresTwoFactor = false,
                Message = _localizer["VerificationSuccess"].Value,
                UserToken = token
            }
        );
    }

    [HttpPost("sign-in/auth2")]
    public async Task<IActionResult> ConfirmCodeSignIn([FromBody] OTPRequest code)
    {
        string userAgent = Request.Headers["User-Agent"].ToString();

        string? currentEmail = await _emailService.GetValidateEmail(userAgent);
        if (currentEmail == null) return BadRequest(
            new LoginResponse
            {
                RequiresTwoFactor = false,
                Message = _localizer["InvalidInformation"].Value
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
                    Message = _localizer["InvalidInformation"].Value
                }
                
            );

        var user = await _authService.VerifyCodeAsync(request);
        if (user == null)
        {
            return Unauthorized(
                new LoginResponse
                {
                    RequiresTwoFactor = false,
                    Message = _localizer["OTPVerificationFailed"].Value
                }
                        
            );
        }

        TokenResponse token = await _tokenService.GenerateTokenAsync(user);

        return Ok(
            new LoginResponse
            {
                RequiresTwoFactor = false,
                Message = _localizer["VerificationSuccess"].Value,
                UserToken = token
            }
        );
    }

}