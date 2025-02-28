using System.ComponentModel.DataAnnotations;
using HUBTSOCIAL.Src.Features.Chat.Models;

namespace HUBT_Social_API.Features.Chat.DTOs;
public class UpdateNickNameRequest
{
    [Required]
    public string GroupId { get; set; } = String.Empty;
    [Required]
    public string UserId { get; set; } = String.Empty;
    public string NewNickName { get; set; } = String.Empty;
}
public class UpdateGroupNameRequest
{
    [Required]
    public string Id { get; set; } = String.Empty;
    public string NewName { get; set; } = String.Empty;
}
public class UpdateAvatarRequest
{
    [Required]
    public string Id { get; set; } = String.Empty;
    public FormFile file { get; set; } 
}

    