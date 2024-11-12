using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Features.Auth.Controllers;

public partial class AccountController
{
    [HttpPost("sign-in")]
    public async Task<IActionResult> LoginAsync(LoginByUserNameRequest model)
    {
        var (result, user) = await _authService.LoginAsync(model);

        if (result.RequiresTwoFactor && user?.Email is not null)
        {
            
            Postcode? code = await _emailService.CreatePostcodeAsync(user.Email);
            if (code == null) return BadRequest(
                LocalValue.Get(KeyStore.InvalidCredentials)
            );

            await _emailService.SendEmailAsync(
                new EmailRequest
                {
                    Code = code.Code,
                    Subject = LocalValue.Get(KeyStore.EmailVerificationCodeSubject),
                    ToEmail = user.Email
                }
            );
            
            
            return Ok(
                new 
                {
                    twoFactor = true,
                    message = LocalValue.Get(KeyStore.StepOneVerificationSuccess),
                    accessToken = ""
                }
                
            );
        }

        if (result.IsLockedOut)
            return BadRequest(
                    LocalValue.Get(KeyStore.AccountLocked)
            );
        if (result.IsNotAllowed)
            return BadRequest(
                LocalValue.Get(KeyStore.LoginNotAllowed)
            );
        if (result.Succeeded && user is not null)
        {
            var token = await _tokenService.GenerateTokenAsync(user);

            return Ok(
                new
                {
                    twoFactor = false,
                    message = LocalValue.Get(KeyStore.VerificationSuccess),
                    accessToken = token
                }
            );
        }
            
        return BadRequest(
            LocalValue.Get(KeyStore.InvalidCredentials)      
        );
    }

}
