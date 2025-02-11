using HUBT_Social_API.Features.Auth.Models;

namespace HUBT_Social_API.Features.Auth.Dtos.Reponse;

public class UserResponse
{
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
    public AUser? User { get; set; }
}