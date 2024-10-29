namespace HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest
{
    public class EmailUpdateRequest
    {
        string UserName { get; set; } = string.Empty;
        string NewEmail { get; set; } = String.Empty;
    }
}