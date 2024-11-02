using System.ComponentModel.DataAnnotations;

namespace HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;

public class UpdateEmailRequest
{
    public string UserName { get; set; } = string.Empty;

    [EmailAddress] public string Email { get; set; } = string.Empty;
}