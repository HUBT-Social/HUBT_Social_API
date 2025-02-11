namespace HUBT_Social_API.Features.Auth.Dtos.Request;

public class PromoteUserRequest
{
    public string UserName { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
}