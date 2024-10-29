namespace HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest
{
    public class UpdateDateOfBornRequest
    {
        public string Username { get; set; } = String.Empty;
        public DateTime DateOfBirth { get; set; }
    }
}
