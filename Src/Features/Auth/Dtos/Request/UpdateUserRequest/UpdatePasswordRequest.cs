using System.ComponentModel.DataAnnotations;

namespace HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;

public class UpdatePasswordRequest
{

    public string NewPassword { get; set; } = string.Empty;
}