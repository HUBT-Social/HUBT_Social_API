using System.ComponentModel.DataAnnotations;

namespace HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest
{
    public class UpdatePhoneNumberRequest
    {
        public string Username { get; set; } = String.Empty;
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
