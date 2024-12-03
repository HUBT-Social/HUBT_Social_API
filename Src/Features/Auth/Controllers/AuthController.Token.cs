
using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Src.Core.Helpers;
using HUBT_Social_API.Src.Features.Auth.Dtos.Reponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Features.Auth.Controllers;


public partial class AuthController
{
    [HttpPut("refresh-token")]
    public async Task<IActionResult> RegenerateToken([FromBody] RefreshTokenRequest refreshToken)
    {
        string? accessToken = TokenHelper.ExtractTokenFromHeader(Request);

        
            
        if (accessToken != null)
        {
            TokenResponse? result = await _tokenService.ValidateTokens(accessToken, refreshToken.RefreshToken);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(LocalValue.Get(KeyStore.InvalidCredentials));
            }
        }
        return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));

    }
    [HttpDelete("delete-token")]
    [Authorize]
    public async Task<IActionResult> DeleteToken()
    {
        string? accessToken = TokenHelper.ExtractTokenFromHeader(Request);
        
        if (!string.IsNullOrEmpty(accessToken))
        {
            UserResponse user = await _tokenService.GetCurrentUser(accessToken);
            if (user.User != null && await _tokenService.DeleteTokenAsync(user.User))
            {
                return Ok(LocalValue.Get(KeyStore.TokenDeleted));
            }
            return BadRequest(LocalValue.Get(KeyStore.InvalidCredentials));

        }

        return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));

    }
}