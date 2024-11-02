namespace HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;

public class UpdateGenderRequest
{
    public string Username { get; set; } = string.Empty;
    public bool IsMale { get; set; }
}