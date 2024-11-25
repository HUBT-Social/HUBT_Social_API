using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Features.Auth.Dtos.Request;

public class UpdateAvatarUrlRequest
{
    [FromForm]
    public string? AvatarUrl { get; set; } = string.Empty;

}