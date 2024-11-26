using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Src.Features.Auth.Dtos.Reponse;

namespace HUBT_Social_API.Features.Auth.Services.Interfaces;

public interface ITokenService
{
    Task<TokenResponse?> GenerateTokenAsync(AUser user);
    Task<TokenResponse?> ValidateTokens(string accessToken,string refreshToken);
    Task<UserResponse> GetCurrentUser(string accessToken);
}