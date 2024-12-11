
using System.ComponentModel.DataAnnotations;

namespace HUBT_Social_API.Features.Chat.DTOs;
public class GetChatHistoryRequest
{
    [Required]
    public string ChatRoomId { get; set; } = string.Empty; 
    public DateTime? Time { get; set; }
    [Required]
    public int Limit { get; set; }
}