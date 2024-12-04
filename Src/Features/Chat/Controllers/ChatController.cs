using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HUBT_Social_API.Features.Chat.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost("send-message")]
    public async Task<IActionResult> SendMessage([FromForm] MessageRequest messageRequest)
    {
        if (messageRequest == null || string.IsNullOrWhiteSpace(messageRequest.SenderId))
            return BadRequest(new { message = "Invalid chat request." });
        
        bool IsSent = await _chatService.SendMessageAsync(messageRequest);
        return IsSent == true
            ? Ok("sent")
            : BadRequest("Sending failed");
    }

    
}