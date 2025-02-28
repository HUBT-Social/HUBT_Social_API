using System.ComponentModel.DataAnnotations;
using HUBTSOCIAL.Src.Features.Chat.Collections;

namespace HUBT_Social_API.Features.Chat.DTOs;

public class UpdateStatusMessageRequest
{
    [Required]
    public string ChatRoomId { get; set; } = string.Empty; // Id của nhóm chat (hoặc cuộc trò chuyện)
    [Required]
    public string MessageId { get; set; } = string.Empty; // Id của tin nhắn cần gỡ
    public MessageActionStatus messageActionStatus { get; set; }
}