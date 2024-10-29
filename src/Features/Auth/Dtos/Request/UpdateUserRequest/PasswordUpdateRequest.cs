namespace HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest
{
    public class PasswordUpdateRequest
    {
        string UserName { get; set; } = string.Empty;
        string NewPassword { get; set; } = String.Empty;
    }
}