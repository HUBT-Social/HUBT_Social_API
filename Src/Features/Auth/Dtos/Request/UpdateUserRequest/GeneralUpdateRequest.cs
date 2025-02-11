using HUBT_Social_API.Core.Settings;

namespace HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;

public class GeneralUpdateRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public Gender Gender { get; set; }

    public DateTime? DateOfBirth { get; set; }
}