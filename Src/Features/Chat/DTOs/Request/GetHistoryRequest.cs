using System.ComponentModel.DataAnnotations;

namespace HUBT_Social_API.Features.Chat.DTOs;
using System;
using System.ComponentModel.DataAnnotations;
using HUBTSOCIAL.Src.Features.Chat.Collections;

public class GetHistoryRequest
{
    [Required]
    public string ChatRoomId { get; set; } = string.Empty;
    public int CurrentQuantity { get; set; }
    public int Limit { get; set; }
    public MessageType? Type { get; set; }
}