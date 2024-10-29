namespace HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest
{
    public class CheckPasswordRequest
    {
        public string Username { get; set; } = String.Empty;
        public string CurrentPassword { get; set; } = String.Empty;
    }
}
