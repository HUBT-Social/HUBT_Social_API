using HUBT_Social_API.Core.Settings;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Features.Auth.Dtos.Request.UpdateUserRequest;

    public class AddInfoUserRequest
    {

        public string AvatarUrl { get; set; } = string.Empty; // URL ảnh hoặc avatar mặc định

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public Gender Gender { get; set; } =0;

        public DateTime DateOfBirth { get; set; } // Ngày sinh
    }