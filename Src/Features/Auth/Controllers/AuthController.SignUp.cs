using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HUBT_Social_API.Features.Auth.Controllers;

public partial class AuthController
{
    [HttpPost("sign-up")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password) || 
                string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {

                return BadRequest(
                    new RegisterResponse
                    {
                        Success = false,
                        Message = LocalValue.Get(KeyStore.DataNotAllowNull)
                    });
            }
            if(request.Password != request.ConfirmPassword)
            {
                return BadRequest(
                    new RegisterResponse
                    {
                        Success = false,
                        Message = LocalValue.Get(KeyStore.ConfirmPasswordError)
                    }
                );
            }
            string? userAgent = Request.Headers.UserAgent.ToString();
            string? ipAddress = TokenHelper.GetIPAddress(HttpContext);
            if (ipAddress == null) return BadRequest(
                new RegisterResponse
                    {
                        Success = false,
                        Message = LocalValue.Get(KeyStore.InvalidInformation)
                    }
                );
            if (!ModelState.IsValid)
                return BadRequest(
                    new RegisterResponse
                    {
                        Success = false,
                        Message = LocalValue.Get(KeyStore.InvalidInformation)
                    });
            if (await _registerService.CheckUserAccountExit(request))
                return BadRequest(
                    new RegisterResponse
                    {
                        Success = false,
                        Message = LocalValue.Get(KeyStore.UserAlreadyExists)
                    });
            if (!await _registerService.AddToTempUser(request))
                return BadRequest(
                    new RegisterResponse
                    {
                        Success = false,
                        Message = LocalValue.Get(KeyStore.UnableToStoreInDatabase)
                    });

            // Gửi mã OTP qua email để xác thực
            try
            {
                Postcode? code = await _emailService.CreatePostcodeAsync(userAgent,request.Email,ipAddress.ToString());
                if (code == null) return BadRequest(
                    new RegisterResponse
                    {
                        Success = false,
                        Message = LocalValue.Get(KeyStore.InvalidCredentials)
                    });

                await _emailService.SendEmailAsync(new EmailRequest
                    { 
                        Code = code.Code, 
                        Subject = LocalValue.Get(KeyStore.EmailVerificationCodeSubject), 
                        ToEmail = request.Email 
                    });
            }
            catch (Exception)
            {
                return BadRequest(
                    new RegisterResponse
                    {
                        Success = false,
                        Message = LocalValue.Get(KeyStore.UnableToSendOTP)
                    });
            }

            return Ok(
                new RegisterResponse
                    {
                        Success = true,
                        Message = LocalValue.Get(KeyStore.RegistrationSuccess)
                    });
        }
        catch(Exception ex)
        {
            // Ghi log lỗi
            // LogError(ex);

            // Trả về lỗi server
            
            return BadRequest(
                new RegisterResponse
                    {
                        Success = true,
                        Message = LocalValue.Get(KeyStore.DefaultLoginError)
                    });
        }
        
    }
}
