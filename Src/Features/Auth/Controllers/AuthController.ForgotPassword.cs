
using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Src.Core.Helpers;
using HUBT_Social_API.Src.Core.Settings;
using HUBT_Social_API.Src.Features.Auth.Dtos.Reponse;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Features.Auth.Controllers;


public partial class AuthController
{
    [HttpPost("forgot-password")]
    public async Task<IActionResult> SearchByUserNameOrEmail([FromBody] SearchUserByUserNameOrPasswordRequest request)
    {
        bool isEmail = request.UserNameOrEmail.Contains("@gmail.com");
        string? userAgent = Request.Headers.UserAgent.ToString();
        string? ipAddress = ServerHelper.GetIPAddress(HttpContext);
        if (ipAddress == null) return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
        
        AUser? user;

        if (isEmail)
        {
            user = await _userService.FindUserByEmailAsync(request.UserNameOrEmail);
        }
        else
        {
            user = await _userService.FindUserByUserNameAsync(request.UserNameOrEmail);
        }

        try
        {
            if (user != null && user.EmailConfirmed)
            {
                Postcode? code = await _emailService.CreatePostcodeForgetPasswordAsync(userAgent, user.Email, ipAddress);
                if (code == null) return BadRequest(LocalValue.Get(KeyStore.InvalidCredentials));

                await _emailService.SendEmailAsync(new EmailRequest
                {
                    Code = code.Code,
                    Subject = LocalValue.Get(KeyStore.EmailVerificationCodeSubject),
                    ToEmail = user.Email,
                    FullName = user.FirstName + " " + user.LastName,
                    Device = userAgent,
                    Location = await ServerHelper.GetLocationFromIpAsync(ipAddress),
                    DateTime = ServerHelper.ConvertToCustomString(DateTime.UtcNow)
                });
                if (_emailService.MaskEmail(user.Email, out string maskEmail))
                {
                    return Ok(
                        new
                        {
                            Email = maskEmail,
                            Message = LocalValue.Get(KeyStore.OtpSent)
                        });
                }
            }
            
        }
        catch (Exception)
        {
            return BadRequest(    
                LocalValue.Get(KeyStore.UserNotFound)
            );
        }

        return NotFound(LocalValue.Get(KeyStore.UserNotFound));


    }
    [HttpPost("forgot-password/password-verification")]
    public async Task<IActionResult> VerifyOtp([FromBody] OTPRequest request)
    {
        string userAgent = Request.Headers.UserAgent.ToString();
        string? ipAddress = ServerHelper.GetIPAddress(HttpContext);
        if (ipAddress == null) return BadRequest(
            new LoginResponse
            {
                Message = LocalValue.Get(KeyStore.InvalidInformation)
            }
            );

        Postcode? currentPostcode = await _emailService.GetCurrentPostCode(userAgent, ipAddress);

        if (currentPostcode == null)
            return BadRequest(LocalValue.Get(KeyStore.UserNotFound));

        var result = await _emailService.ValidatePostcodeAsync(new ValidatePostcodeRequest
        {
            Postcode = request.Postcode,
            Email = currentPostcode.Email,
            UserAgent = userAgent
        });
        if (result is not null)
        {
            if (currentPostcode.ExtendPostcodeExpireTime(10))
                await _emailService.UpdatePostcode(currentPostcode);
            return Ok(LocalValue.Get(KeyStore.OtpVerificationSuccess));
        }
        return BadRequest(LocalValue.Get(KeyStore.OtpInvalid));
    }
    [HttpPut("forgot-password/password-verification/resend")]
    public async Task<IActionResult> ResendForgetPasswordPostcode()
    {
        return await PostcodeHelper.ResendPostcode(HttpContext, Request, _emailService.CreatePostcodeForgetPasswordAsync, _emailService, PostcodeType.ForgetPassword);
    }
    [HttpPut("forgot-password/change-password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
    {
        string? userAgent = Request.Headers.UserAgent.ToString();
        string? ipAddress = ServerHelper.GetIPAddress(HttpContext);
        if (ipAddress == null) return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
        Postcode? currentEmail = await _emailService.GetCurrentPostCode(userAgent, ipAddress);
        if (ModelState.IsValid && currentEmail != null && currentEmail.PostcodeType == PostcodeType.ForgetPassword)
        {

            AUser? User = await _userService.FindUserByEmailAsync(currentEmail.Email);
            if (User != null)
                return await UpdateHelper.HandleUserUpdateForgotPassword(KeyStore.PasswordUpdated, KeyStore.PasswordUpdateError, _userService.UpdatePasswordAsync, request, User);
            return BadRequest(LocalValue.Get(KeyStore.UserNotFound));
        }

        return BadRequest(LocalValue.Get(KeyStore.ConfirmPasswordError));
    }

}
