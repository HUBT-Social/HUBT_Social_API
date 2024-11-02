using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Models;

namespace HUBT_Social_API.Features.Auth.Services.Interfaces;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(AUser user);
    DecodeTokenResponse ValidateToken(string accessToken);
    Task<UserResponse> GetCurrentUser(string accessToken);
}