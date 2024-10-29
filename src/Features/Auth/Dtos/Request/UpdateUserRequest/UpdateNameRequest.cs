namespace HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest
{
    public class UpdateNameRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = String.Empty;
        public string LastName { get; set; } = String.Empty;
    }
}