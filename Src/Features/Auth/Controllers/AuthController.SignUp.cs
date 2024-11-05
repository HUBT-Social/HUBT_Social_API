using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Features.Auth.Controllers;

public partial class AccountController
{
    [HttpPost("sign-up")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(
                new AuthResponse(
                    _localizer["InvalidInformation"]
                )
            );
        if (await _registerService.CheckUserAccountExit(request))
            return BadRequest(
                new AuthResponse(
                    _localizer["UserAlreadyExists"]
                )
            );
        if (!await _registerService.AddToTempUser(request))
            return BadRequest(
                new AuthResponse(
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
                    _localizer["UnableToSendOTP"]
                )
            );
        }

        return Ok(
            new AuthResponse(
                _localizer["RegistrationSuccess"]
            )
        );
    }
}
