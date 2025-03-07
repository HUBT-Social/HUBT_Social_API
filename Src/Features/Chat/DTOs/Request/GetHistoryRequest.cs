using System.ComponentModel.DataAnnotations;
using HUBTSOCIAL.Src.Features.Chat.Collections;

namespace HUBT_Social_API.Features.Chat.DTOs;

public class GetHistoryRequest
{
    [Required]
    public string ChatRoomId { get; set; } = string.Empty;
    public int CurrentQuantity { get; set; }
    public int Limit { get; set; }
    public MessageType? Type { get; set; }
}