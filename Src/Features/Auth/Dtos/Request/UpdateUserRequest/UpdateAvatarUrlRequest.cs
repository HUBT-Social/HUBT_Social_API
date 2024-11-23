using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Features.Auth.Dtos.Request;

public class UpdateAvatarUrlRequest
{
    [FromForm]
    public IFormFile? file { get; set; } // File được chọn để upload
    public string? AvatarUrl { get; set; } = string.Empty;

}