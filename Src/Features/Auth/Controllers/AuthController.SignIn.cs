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
                _localizer["InvalidCredentials"].Value
            );

            await _emailService.SendEmailAsync(
                new EmailRequest
                {
                    Code = code.Code,
                    Subject = _localizer["EmailVerificationCodeSubject"].Value,
                    ToEmail = user.Email
                }
            );
            
            
            return Ok(
                new 
                {
                    twoFactor = true,
                    message = _localizer["StepOneVerificationSuccess"].Value,
                    accessToken = ""
                }
                
            );
        }

        if (result.IsLockedOut)
            return BadRequest(
                    _localizer["AccountLocked"].Value
            );
        if (result.IsNotAllowed)
            return BadRequest(
                _localizer["LoginNotAllowed"].Value
            );
        if (result.Succeeded && user is not null)
        {
            var token = await _tokenService.GenerateTokenAsync(user);

            return Ok(
                new
                {
                    twoFactor = false,
                    message = _localizer["VerificationSuccess"].Value,
                    accessToken = token
                }
            );
        }
            
        return BadRequest(
            _localizer["InvalidCredentials"].Value      
        );
    }

}
