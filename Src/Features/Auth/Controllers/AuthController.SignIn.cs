using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;
using HUBT_Social_API.Features.Auth.Services;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Features.Auth.Controllers;

public partial class AccountController
{
    [HttpPost("sign-in")]
    public async Task<IActionResult> LoginAsync(LoginByUserNameRequest model)
    {
        string? userAgent = Request.Headers.UserAgent.ToString();
        string? ipAddress = TokenHelper.GetIPAddress(HttpContext);
        if (ipAddress == null) return BadRequest(
            new LoginResponse
            {
                RequiresTwoFactor = false,
                Message = LocalValue.Get(KeyStore.LoginNotAllowed),
            }
        );

        var (result, user) = await _authService.LoginAsync(model);

        if (result.RequiresTwoFactor && user?.Email is not null)
        {
            
            Postcode? code = await _emailService.CreatePostcodeAsync(userAgent,user.Email,ipAddress.ToString());
            if (code == null) return BadRequest(
                new LoginResponse
                {
                    RequiresTwoFactor = true,
                    Message = LocalValue.Get(KeyStore.InvalidCredentials)
                }        
            );

            await _emailService.SendEmailAsync(
                new EmailRequest
                {
                    ToEmail =  code.Email,
                    Code = code.Code,
                    Subject = LocalValue.Get(KeyStore.EmailVerificationCodeSubject)
                }
            );
            
            
            return Ok(
                new LoginResponse 
                {
                    RequiresTwoFactor = true,
                    Message = LocalValue.Get(KeyStore.StepOneVerificationSuccess)
                }
                
            );
        }

        if (result.IsLockedOut)
            return BadRequest(new LoginResponse
            {
                RequiresTwoFactor = false,
                Message = LocalValue.Get(KeyStore.AccountLocked)
            }       
            );
        if (result.IsNotAllowed)
            return BadRequest(
                new LoginResponse
                {
                    RequiresTwoFactor = false,
                    Message = LocalValue.Get(KeyStore.LoginNotAllowed),
                }
            );
        if (result.Succeeded && user is not null)
        {
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
            
        return BadRequest(
            new LoginResponse
            {
                RequiresTwoFactor = true,
                Message = LocalValue.Get(KeyStore.InvalidCredentials)
            }

        );
    }

}
