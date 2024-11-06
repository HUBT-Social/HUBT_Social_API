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
            try
            {
                var code = await _emailService.CreatePostcodeAsync(user.Email);

                await _emailService.SendEmailAsync(
                    new EmailRequest
                    {
                        Code = code.Code,
                        Subject = _localizer["EmailVerificationCodeSubject"].Value,
                        ToEmail = user.Email
                    });
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    new {
                        message = _localizer["UnableToSendOTP"].Value
                    }
                );
            }

            return Ok(
                new
                {
                    message = _localizer["StepOneVerificationSuccess"].Value
                }

            );
        }

        if (result.IsLockedOut)
            return BadRequest(
                new {
                    message = _localizer["AccountLocked"].Value
                }
            );
        if (result.IsNotAllowed)
            return BadRequest(
                new
                {
                    message =_localizer["LoginNotAllowed"].Value
                }
            );
        if (result.Succeeded)
        {
            var token = await _tokenService.GenerateTokenAsync(user);

            return Ok(
                new
                {
                    message = _localizer["VerificationSuccess"].Value,
                    accessToken = token
                }
            );
        }
            
        return BadRequest(
            new
            {
                message = _localizer["InvalidCredentials"].Value
            }
        );
    }

}
