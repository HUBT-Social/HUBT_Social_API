using MongoDB.Driver.Core.Authentication;

namespace HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest
{
    public class GeneralUpdateRequest
    {
        
        public string Username { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string FirstName { get; set; } = String.Empty;
        public string LastName { get; set; } = String.Empty;
        public string PhoneNumber { get; set; } = String.Empty;

        public  bool IsMale { get; set; } = false;

        public DateTime DateOfBirth { get; set; }

    }
}