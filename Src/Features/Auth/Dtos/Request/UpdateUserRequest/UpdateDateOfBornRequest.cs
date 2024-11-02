namespace HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;

public class UpdateDateOfBornRequest
{
    public string Username { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
}