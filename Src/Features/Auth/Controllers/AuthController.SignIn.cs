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
                        Subject = _localizer["EmailVerificationCodeSubject"],
                        ToEmail = user.Email
                    });
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    new {
                        message = _localizer["UnableToSendOTP"]
                    }
                );
            }

            return Ok(
                new
                {
                    message = _localizer["StepOneVerificationSuccess"]
                }

            );
        }

        if (result.IsLockedOut)
            return BadRequest(
                new {
                    message = _localizer["AccountLocked"]
                }
            );
        if (result.IsNotAllowed)
            return BadRequest(
                new
                {
                    message =_localizer["LoginNotAllowed"]
                }
            );
        if (result.Succeeded)
        {
            var token = await _tokenService.GenerateTokenAsync(user);

            return Ok(
                new
                {
                    message = _localizer["VerificationSuccess"],
                    accessToken = token
                }
            );
        }
            
        return BadRequest(
            new
            {
                message = _localizer["InvalidCredentials"]
            }
        );
    }

}
