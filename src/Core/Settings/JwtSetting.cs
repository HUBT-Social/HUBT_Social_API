namespace HUBT_Social_API.src.Core.Settings
{
    public class JwtSetting
    {
        public string SecretKey { get; set; } = String.Empty;
        public string RefreshSecretKey { get; set; } = String.Empty;
        public string Issuer { get; set; } = String.Empty;
        public string Audience { get; set; } = String.Empty;
        public int TokenExpirationInMinutes { get; set; }
        public int RefreshTokenExpirationInDays { get; set; }
    }
}
