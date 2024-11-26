
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
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RegenerateToken(string refreshToken)
    {
        TokenResponse? result = await _tokenService.ValidateTokens(refreshToken);
        if (result != null)
        {
            return Ok(result);
        }
        
        return BadRequest(
            LocalValue.Get(KeyStore.InvalidCredentials));
    }
}