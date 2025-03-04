using System.ComponentModel.DataAnnotations;

namespace HUBT_Social_API.Features.Chat.DTOs;
using System;
using System.ComponentModel.DataAnnotations;
using HUBTSOCIAL.Src.Features.Chat.Collections;

public class GetHistoryRequest
{
    /// <summary>
    /// The unique identifier of the chat room.
    /// </summary>
    [Required]
    public string ChatRoomId { get; set; } = string.Empty;
    public string LastBlockId { get; set; } = string.Empty;
    //public int Limit { get; set; } = 20;
    public MessageType? Type { get; set; } = MessageType.All;
    
}