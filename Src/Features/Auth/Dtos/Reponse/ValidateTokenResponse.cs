using HUBT_Social_API.Features.Auth.Dtos.Reponse;

namespace HUBT_Social_API.Src.Features.Auth.Dtos.Reponse
{
    public class ValidateTokenResponse
    {
        public bool AccessTokenIsValid { get; set; }
        public bool RefreshTokenIsValid { get; set; }

        public string Message { get; set; } = string.Empty;

        public TokenResponse? NewTokens { get; set; }
    }
}
