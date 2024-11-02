namespace HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;

public class GeneralUpdateRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public bool IsMale { get; set; } = false;

    public DateTime DateOfBirth { get; set; }
}