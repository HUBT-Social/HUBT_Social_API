namespace HUBT_Social_API.Features.Auth.Dtos.Reponse;

public class LoginResponse
{
    public TokenResponse? UserToken { get; set; } = null;

    public string Message { get; set; } = string.Empty;
    public bool RequiresTwoFactor { get; set; }
}