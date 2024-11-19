using HUBT_Social_API.Core.Settings;

namespace HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;

    public class AddInfoUserRequest
    {
        public string AvatarUrl { get; set; } = string.Empty; // Bên thiết kế mục này sẽ là muc chọn ảnh, nếu không chọ thì tôi sẽ random avatar
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public Gender Gender { get; set; }

        public DateTime DateOfBirth { get; set; }
    }