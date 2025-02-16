using HUBT_Social_API.Core.Settings;

namespace HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;

public class GeneralUpdateRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateTime DateOfBirth { get; set; } 
}