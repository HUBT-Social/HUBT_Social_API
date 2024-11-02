namespace HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;

public class CheckPasswordRequest
{
    public string Username { get; set; } = string.Empty;
    public string CurrentPassword { get; set; } = string.Empty;
}