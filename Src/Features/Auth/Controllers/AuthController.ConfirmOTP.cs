using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Features.Auth.Controllers;

public partial class AccountController
{
    [HttpPost("sign-up/confirm-code")]
    public async Task<IActionResult> ConfirmCodeSignUp([FromBody] ValidatePostcodeRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(
                new
                {
                    message = _localizer["InvalidInformation"].Value
                }
            );

        
        var tempUser = await _authService.GetTempUser(request.Email);
        if (tempUser == null)
            return Unauthorized(
                new
                {
                    message = _localizer["OTPVerificationFailed"].Value
                });

        var (result, registeredUser) = await _authService.RegisterAsync(new RegisterRequest
        {
            Email = tempUser.Email,
            Password = tempUser.Password,
            UserName = tempUser.UserName
        });

        if (!result.Succeeded || registeredUser is null)
            return Unauthorized(
                new
                {
                    message = _localizer["OTPVerificationFailed"].Value
                });

        var user = await _authService.VerifyCodeAsync(request);
        if (user == null)
        {
            return Unauthorized(
                    new
                    {
                        message = _localizer["OTPVerificationFailed"].Value
                    });
        }

        var token = await _tokenService.GenerateTokenAsync(user);

        return Ok(
            new
            {
                message = _localizer["VerificationSuccess"].Value,
                accessToken = token
            }
        );
    }

    [HttpPost("sign-in/confirm-code")]
    public async Task<IActionResult> ConfirmCodeSignIn([FromBody] ValidatePostcodeRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(
                new
                {
                    message = _localizer["InvalidInformation"].Value
                }
            );

        var user = await _authService.VerifyCodeAsync(request);
        if (user == null)
        {
            return Unauthorized(
                new
                {
                    message = _localizer["OTPVerificationFailed"].Value
                }        
            );
        }

        var token = await _tokenService.GenerateTokenAsync(user);

        return Ok(
            new
            {
                message = _localizer["VerificationSuccess"].Value,
                accessToken = token
            }
        );
    }

}