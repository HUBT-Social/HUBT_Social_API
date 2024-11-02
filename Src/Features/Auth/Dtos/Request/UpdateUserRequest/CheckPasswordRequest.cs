namespace HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;

public class CheckPasswordRequest
{
    public string CurrentPassword { get; set; } = string.Empty;
}