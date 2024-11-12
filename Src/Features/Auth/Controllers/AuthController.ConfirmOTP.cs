using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Features.Auth.Controllers;

public partial class AccountController
{
    [HttpPost("sign-up/verify-two-factor")]
    public async Task<IActionResult> ConfirmCodeSignUp([FromBody] ValidatePostcodeRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(
                LocalValue.Get(KeyStore.InvalidInformation)
            );

        
        var tempUser = await _authService.GetTempUser(request.Email);
        if (tempUser == null)
            return Unauthorized(
                 LocalValue.Get(KeyStore.OTPVerificationFailed)
                );

        var (result, registeredUser) = await _authService.RegisterAsync(new RegisterRequest
        {
            Email = tempUser.Email,
            Password = tempUser.Password,
            UserName = tempUser.UserName
        });

        if (!result.Succeeded || registeredUser is null)
            return Unauthorized(
                    LocalValue.Get(KeyStore.OTPVerificationFailed)
                );

        var user = await _authService.VerifyCodeAsync(request);
        if (user == null)
        {
            return Unauthorized(
                    LocalValue.Get(KeyStore.OTPVerificationFailed)
                    );
        }

        var token = await _tokenService.GenerateTokenAsync(user);

        return Ok(
            new
            {
                message = LocalValue.Get(KeyStore.VerificationSuccess),
                accessToken = token
            }
        );
    }

    [HttpPost("sign-in/verify-two-factor")]
    public async Task<IActionResult> ConfirmCodeSignIn([FromBody] ValidatePostcodeRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(
                  LocalValue.Get(KeyStore.InvalidInformation)
            );

        var user = await _authService.VerifyCodeAsync(request);
        if (user == null)
        {
            return Unauthorized(
                LocalValue.Get(KeyStore.OTPVerificationFailed)        
            );
        }

        var token = await _tokenService.GenerateTokenAsync(user);

        return Ok(
            new
            {
                message = LocalValue.Get(KeyStore.VerificationSuccess),
                accessToken = token
            }
        );
    }

}