using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;
using HUBT_Social_API.Src.Core.Helpers;
using HUBT_Social_API.Src.Core.Settings;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Features.Auth.Controllers;

public partial class AuthController
{
    [HttpPost("sign-in")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginByUserNameRequest model)
    {
        try
        {
            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
                return BadRequest(
                    new LoginResponse
                    {
                        RequiresTwoFactor = false,
                        Message =
                            $"{LocalValue.Get(KeyStore.EmailCannotBeEmpty)} or {LocalValue.Get(KeyStore.PasswordCannotBeEmpty)}"
                    });

            // Lấy User-Agent từ Header
            var userAgent = Request.Headers.UserAgent.ToString();
            //Lấy Ip
            var ipAddress = ServerHelper.GetIPAddress(HttpContext);
            if (ipAddress == null)
                return BadRequest(
                    new LoginResponse
                    {
                        RequiresTwoFactor = false,
                        Message = LocalValue.Get(KeyStore.LoginNotAllowed)
                    }
                );

            // Thực hiện đăng nhập
            var (result, user) = await _authService.LoginAsync(model);

            // Kiểm tra yêu cầu xác thực hai yếu tố
            if (result.RequiresTwoFactor && user?.Email is not null)
            {
                var code = await _emailService.CreatePostcodeSignInAsync(userAgent, user.Email, ipAddress);
                if (code != null && _emailService.MaskEmail(user.Email, out var maskEmail))
                {
                    await _emailService.SendEmailAsync(new EmailRequest
                    {
                        ToEmail = code.Email,
                        Code = code.Code,
                        Subject = LocalValue.Get(KeyStore.EmailVerificationCodeSubject),
                        FullName = user.FirstName + " " + user.LastName,
                        Device = userAgent,
                        Location = await ServerHelper.GetLocationFromIpAsync(ipAddress),
                        DateTime = ServerHelper.ConvertToCustomString(DateTime.UtcNow)
                    });

                    return Ok(new LoginResponse
                    {
                        RequiresTwoFactor = true,
                        MaskEmail = maskEmail,
                        Message = LocalValue.Get(KeyStore.StepOneVerificationSuccess)
                    });
                }

                return BadRequest(new LoginResponse
                {
                    RequiresTwoFactor = true,
                    Message = LocalValue.Get(KeyStore.InvalidCredentials)
                });
            }

            // Xử lý các trường hợp đăng nhập thất bại
            if (result.IsLockedOut)
                return BadRequest(new LoginResponse
                {
                    RequiresTwoFactor = false,
                    Message = LocalValue.Get(KeyStore.AccountLocked)
                });

            if (result.IsNotAllowed)
                return BadRequest(new LoginResponse
                {
                    RequiresTwoFactor = false,
                    Message = LocalValue.Get(KeyStore.LoginNotAllowed)
                });

            // Xử lý đăng nhập thành công
            if (result.Succeeded && user is not null)
            {
                var token = await _tokenService.GenerateTokenAsync(user);

                return Ok(new LoginResponse
                {
                    RequiresTwoFactor = false,
                    Message = LocalValue.Get(KeyStore.VerificationSuccess),
                    UserToken = token
                });
            }

            // Mặc định trả về lỗi đăng nhập
            return BadRequest(new LoginResponse
            {
                RequiresTwoFactor = false,
                Message = LocalValue.Get(KeyStore.InvalidCredentials)
            });
        }
        catch (Exception ex)
        {
            // Ghi log lỗi
            // LogError(ex);

            // Trả về lỗi server
            return BadRequest(
                new LoginResponse
                {
                    RequiresTwoFactor = false,
                    Message = LocalValue.Get(KeyStore.DefaultLoginError)
                });
        }
    }

    [HttpPost("sign-in/verify-two-factor")]
    public async Task<IActionResult> ConfirmCodeSignIn([FromBody] OTPRequest code)
    {
        var userAgent = Request.Headers.UserAgent.ToString();
        var ipAddress = ServerHelper.GetIPAddress(HttpContext);
        if (ipAddress == null)
            return BadRequest(
                new LoginResponse
                {
                    RequiresTwoFactor = false,
                    Message = LocalValue.Get(KeyStore.InvalidInformation)
                }
            );
        var currentEmail = await _emailService.GetCurrentPostCode(userAgent, ipAddress);
        if (currentEmail == null)
            return BadRequest(
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

        var user = await _authService.VerifyCodeAsync(request);
        if (user == null)
            return Unauthorized(
                new LoginResponse
                {
                    RequiresTwoFactor = false,
                    Message = LocalValue.Get(KeyStore.OTPVerificationFailed)
                }
            );

        var token = await _tokenService.GenerateTokenAsync(user);

        return Ok(
            new LoginResponse
            {
                RequiresTwoFactor = false,
                Message = LocalValue.Get(KeyStore.VerificationSuccess),
                UserToken = token
            }
        );
    }

    [HttpPut("sign-in/verify-two-factor/resend")]
    public async Task<IActionResult> ResendSignInPostcode()
    {
        return await PostcodeHelper.ResendPostcode(HttpContext, Request, _emailService.CreatePostcodeSignInAsync,
            _emailService, PostcodeType.SignIn);
    }
}