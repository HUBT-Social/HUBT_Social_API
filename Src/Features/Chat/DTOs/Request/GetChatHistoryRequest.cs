
using System.ComponentModel.DataAnnotations;

namespace HUBT_Social_API.Features.Chat.DTOs;
using System;
using System.ComponentModel.DataAnnotations;

public class GetChatHistoryRequest
{
    /// <summary>
    /// The unique identifier of the chat room.
    /// </summary>
    [Required]
    public string ChatRoomId { get; set; } = string.Empty;

    /// <summary>
    /// The optional timestamp to filter messages before or after this time.
    /// </summary>
    public DateTime? Time { get; set; }

    /// <summary>
    /// The maximum number of chat messages to fetch.
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Limit must be greater than 0.")]
    public int Limit { get; set; }
}