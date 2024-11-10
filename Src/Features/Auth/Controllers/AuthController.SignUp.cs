using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Features.Auth.Controllers;

public partial class AccountController
{
    [HttpPost("sign-up")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        string? userAgent = Request.Headers["User-Agent"].ToString();

        if (!ModelState.IsValid)
            return BadRequest(_localizer["InvalidInformation"].Value);
        if (await _registerService.CheckUserAccountExit(request))
            return BadRequest(_localizer["UserAlreadyExists"].Value);
        if (!await _registerService.AddToTempUser(request))
            return BadRequest(_localizer["UnableToStoreInDatabase"].Value);

        // Gửi mã OTP qua email để xác thực
        try
        {
            Postcode? code = await _emailService.CreatePostcodeAsync(userAgent,request.Email);
            if (code == null) return BadRequest(_localizer["InvalidCredentials"].Value);

            await _emailService.SendEmailAsync(new EmailRequest
                { Code = code.Code, Subject = _localizer["EmailVerificationCodeSubject"].Value, ToEmail = request.Email });
        }
        catch (Exception)
        {
            return StatusCode(
                500,
                _localizer["UnableToSendOTP"].Value
            );
        }

        return Ok(
             _localizer["RegistrationSuccess"].Value
        );
    }
}
