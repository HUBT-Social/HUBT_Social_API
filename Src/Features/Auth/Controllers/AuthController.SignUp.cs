using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Services;
using HUBT_Social_API.Src.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HUBT_Social_API.Features.Auth.Controllers;

public partial class AuthController
{
    [HttpPost("sign-up")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        string? userAgent = Request.Headers.UserAgent.ToString();
        string? ipAddress = ServerHelper.GetIPAddress(HttpContext);
        if (ipAddress == null) return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
        if (!ModelState.IsValid)
            return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
        if (await _registerService.CheckUserAccountExit(request))
            return BadRequest(LocalValue.Get(KeyStore.UserAlreadyExists));
        if (!await _registerService.AddToTempUser(request))
            return BadRequest(LocalValue.Get(KeyStore.UnableToStoreInDatabase));

        // Gửi mã OTP qua email để xác thực
        try
        {
            Postcode? code = await _emailService.CreatePostcodeAsync(userAgent, request.Email, ipAddress.ToString());
            if (code == null) return BadRequest(LocalValue.Get(KeyStore.InvalidCredentials));

            await _emailService.SendEmailAsync(new EmailRequest
            {
                Code = code.Code,
                Subject = LocalValue.Get(KeyStore.EmailVerificationCodeSubject),
                ToEmail = request.Email
            });
        }
        catch (Exception)
        {
            return StatusCode(
                500,
                LocalValue.Get(KeyStore.UnableToSendOTP)
            );
        }

        return Ok(
             LocalValue.Get(KeyStore.RegistrationSuccess)
        );
    }
}
        
