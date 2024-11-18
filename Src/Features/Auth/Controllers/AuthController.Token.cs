
using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Src.Features.Auth.Dtos.Reponse;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Features.Auth.Controllers;


public partial class AccountController
{

    [HttpGet("token-is-validate")]
    public async Task<IActionResult> ValidateToken()
    {
        string token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

        ValidateTokenResponse result = await _tokenService.ValidateTokens(token);
        if (token != null)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RegenerateToken()
    {
        string token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

        UserResponse userResponse = await _tokenService.GetCurrentUser(token);

        if (userResponse.User is not null)
        {
            TokenResponse tokenResponse = await _tokenService.GenerateTokenAsync(userResponse.User);
            return Ok(tokenResponse);
        }

        return BadRequest();
    }
}