namespace HUBT_Social_API.src.Features.Auth.Dtos.Reponse
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}
