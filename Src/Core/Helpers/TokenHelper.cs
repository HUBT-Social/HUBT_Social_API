using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Services.Interfaces;

namespace HUBT_Social_API.Src.Core.Helpers;

public static class TokenHelper
{
    public static string? ExtractTokenFromHeader(HttpRequest request)
    {
        var token = request.Headers.Authorization.ToString();
        return !string.IsNullOrEmpty(token) ? token.Replace("Bearer ", "") : null;
    }

    public static async Task<UserResponse> GetUserResponseFromToken(HttpRequest request, ITokenService tokenService)
    {
        var token = ExtractTokenFromHeader(request);
        if (string.IsNullOrEmpty(token))
            return new UserResponse { Success = false };

        return await tokenService.GetCurrentUser(token);
    }
}