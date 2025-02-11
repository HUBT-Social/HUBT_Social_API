using HUBT_Social_API.Core.Settings;

namespace HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;

public class AddInfoUserRequest
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public Gender Gender { get; set; } = 0;

    public DateTime DateOfBirth { get; set; } // Ngày sinh
}