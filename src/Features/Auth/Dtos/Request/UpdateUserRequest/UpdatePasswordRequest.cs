using System.ComponentModel.DataAnnotations;

namespace HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest
{
    public class UpdatePasswordRequest
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        public string NewPassword { get; set; } = String.Empty;
    }
}