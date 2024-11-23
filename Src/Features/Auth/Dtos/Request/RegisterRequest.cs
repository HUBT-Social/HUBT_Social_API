using System.ComponentModel.DataAnnotations;

namespace HUBT_Social_API.Features.Auth.Dtos.Request;

public class RegisterRequest
{
    public string UserName { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;

    [EmailAddress] 
    public string Email { get; set; } = string.Empty;
}