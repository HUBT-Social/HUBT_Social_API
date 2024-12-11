using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services;
using HUBT_Social_API.src.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Src.Core.Helpers;
using HUBT_Social_API.Src.Core.Settings;
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

        try
        {
            Postcode? code = await _emailService.CreatePostcodeSignUpAsync(userAgent, request.Email, ipAddress.ToString());
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

    [HttpPost("sign-up/verify-otp")]
    public async Task<IActionResult> ConfirmCodeSignUp([FromBody] OTPRequest code)
    {
        string userAgent = Request.Headers.UserAgent.ToString();
        string? ipAddress = ServerHelper.GetIPAddress(HttpContext);
        if (ipAddress == null) return BadRequest(
            new LoginResponse
            {
                RequiresTwoFactor = false,
                Message = LocalValue.Get(KeyStore.InvalidInformation)
            }
            );

        Postcode? currentEmail = await _emailService.GetCurrentPostCode(userAgent, ipAddress);
        if (currentEmail == null) return BadRequest(
            new LoginResponse
            {
                RequiresTwoFactor = false,
                Message = LocalValue.Get(KeyStore.InvalidInformation)
            }
            );

        ValidatePostcodeRequest request = new()
        {
            Postcode = code.Postcode,
            UserAgent = userAgent,
            Email = currentEmail.Email
        };


        if (!ModelState.IsValid)
            return BadRequest(
                new LoginResponse
                {
                    RequiresTwoFactor = false,
                    Message = LocalValue.Get(KeyStore.InvalidInformation)
                }
            );


        TempUserRegister? tempUser = await _authService.GetTempUser(request.Email);
        if (tempUser == null)
            return Unauthorized(
                 new LoginResponse
                 {
                     RequiresTwoFactor = false,
                     Message = LocalValue.Get(KeyStore.OTPVerificationFailed)
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
                Message = LocalValue.Get(KeyStore.OTPVerificationFailed)
            });

        AUser? user = await _authService.VerifyCodeAsync(request);
        if (user == null)
        {
            return Unauthorized(new LoginResponse
            {
                RequiresTwoFactor = false,
                Message = LocalValue.Get(KeyStore.OTPVerificationFailed)
            }
            );
        }

        TokenResponse? token = await _tokenService.GenerateTokenAsync(user);

        return Ok(
            new LoginResponse
            {
                RequiresTwoFactor = false,
                Message = LocalValue.Get(KeyStore.VerificationSuccess),
                UserToken = token
            }
        );
    }
    [HttpPost("sign-up/verify-otp/resend")]
    public async Task<IActionResult> ResendSignUpPostcode()
    {

        return await PostcodeHelper.ResendPostcode(HttpContext, Request, _emailService.CreatePostcodeSignUpAsync, _emailService, PostcodeType.SignUp);
    }
}
        
