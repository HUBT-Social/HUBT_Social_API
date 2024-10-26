


using HUBT_Social_API.src.Features.Authentication.Models;
using HUBT_Social_API.src.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.src.Features.Auth.Dtos.Request;

namespace HUBT_Social_API.src.Features.Auth.Services.IAuthServices
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(AUser user);
        DecodeTokenResponse ValidateToken(string accessToken);
        Task<TokenResponse> RefreshToken(RefreshTokenRequest request);
        Task<UserResponse> GetCurrentUser(string accessToken);

    }
}

