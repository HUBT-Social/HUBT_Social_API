
using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Src.Core.Helpers;
using HUBT_Social_API.Src.Features.Auth.Dtos.Reponse;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Features.Auth.Controllers;


public partial class AuthController
{

    [HttpGet("token-is-validate")]
    public async Task<IActionResult> ValidateToken(string refreshToken)
    {
        
        string? token = TokenHelper.ExtractTokenFromHeader(Request);

        
        if (token != null)
        {
            ValidateTokenResponse result = await _tokenService.ValidateTokens(token, refreshToken);
            return Ok(result);
        }

        return BadRequest(new ValidateTokenResponse
        {
            AccessTokenIsValid = false,
            RefreshTokenIsValid = false,
            Message = LocalValue.Get(KeyStore.InvalidCredentials)
        });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RegenerateToken()
    {
        string? token = TokenHelper.ExtractTokenFromHeader(Request);
        if (token != null)
        {
            UserResponse userResponse = await _tokenService.GetCurrentUser(token);

            if (userResponse.User is not null)
            {
                TokenResponse tokenResponse = await _tokenService.GenerateTokenAsync(userResponse.User);
                return Ok(tokenResponse);
            }
        }
        return BadRequest(new ValidateTokenResponse
        {
            AccessTokenIsValid = false,
            RefreshTokenIsValid = false,
            Message = LocalValue.Get(KeyStore.InvalidCredentials)
        });
    }
}