using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Models;

namespace HUBT_Social_API.Features.Auth.Services.Interfaces;

public interface ITokenService
{
    Task<TokenResponse> GenerateTokenAsync(AUser user);
    Task<DecodeTokenResponse> ValidateTokens(string accessToken);
    Task<AUser?> GetCurrentUser(string accessToken);
}