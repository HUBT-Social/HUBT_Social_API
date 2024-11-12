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
        string? userAgent = Request.Headers["User-Agent"].ToString();
        var (result, user) = await _authService.LoginAsync(model);

        if (result.RequiresTwoFactor && user?.Email is not null)
        {
            
            Postcode? code = await _emailService.CreatePostcodeAsync(userAgent,user.Email);
            if (code == null) return BadRequest(
                new LoginResponse
                {
                    RequiresTwoFactor = true,
                    Message = _localizer["InvalidCredentials"].Value
                }        
            );

            await _emailService.SendEmailAsync(
                new EmailRequest
                {
                    Code = code.Code,
                    Subject = _localizer["EmailVerificationCodeSubject"].Value
                }
            );
            
            
            return Ok(
                new LoginResponse 
                {
                    RequiresTwoFactor = true,
                    Message = _localizer["StepOneVerificationSuccess"].Value
                }
                
            );
        }

        if (result.IsLockedOut)
            return BadRequest(new LoginResponse
            {
                RequiresTwoFactor = false,
                Message = _localizer["AccountLocked"].Value
            }       
            );
        if (result.IsNotAllowed)
            return BadRequest(
                new LoginResponse
                {
                    RequiresTwoFactor = false,
                    Message = _localizer["LoginNotAllowed"].Value,
                }
            );
        if (result.Succeeded && user is not null)
        {
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
            
        return BadRequest(
            new LoginResponse
            {
                RequiresTwoFactor = true,
                Message = _localizer["InvalidCredentials"].Value
            }

        );
    }

}
