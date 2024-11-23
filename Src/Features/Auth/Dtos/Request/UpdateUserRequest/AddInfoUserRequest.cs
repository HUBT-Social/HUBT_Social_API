using HUBT_Social_API.Core.Settings;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;

    public class AddInfoUserRequest
    {
        [FromForm]
        public IFormFile? file { get; set; } // File được chọn để upload

        [FromForm]
        public string? AvatarUrl { get; set; } = string.Empty; // URL ảnh hoặc avatar mặc định

        [FromForm]
        public string FirstName { get; set; } = string.Empty;

        [FromForm]
        public string LastName { get; set; } = string.Empty;

        [FromForm]
        public string PhoneNumber { get; set; } = string.Empty;

        [FromForm]
        public Gender Gender { get; set; } // Enum: Male = 1, Female = 2,...

        [FromForm]
        public DateTime DateOfBirth { get; set; } // Ngày sinh
    }