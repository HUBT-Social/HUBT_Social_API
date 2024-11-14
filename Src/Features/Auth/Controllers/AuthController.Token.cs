
using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Models;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Features.Auth.Controllers;


public partial class AccountController
{

    [HttpGet("token-is-validate")]
    public async Task<IActionResult> ValidateToken()
    {
        string token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

        DecodeTokenResponse result = await _tokenService.ValidateTokens(token);
        if (result.Success)
        {
            return Ok(LocalValue.Get(KeyStore.TokenValid));
        }

        return BadRequest(result.Message);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RegenerateToken()
    {
        string token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

        AUser? user = await _tokenService.GetCurrentUser(token);

        if (user is not null)
        {
            TokenResponse tokenResponse = await _tokenService.GenerateTokenAsync(user);
            return Ok(tokenResponse);
        }

        return BadRequest();
    }
}