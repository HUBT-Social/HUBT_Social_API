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

    private readonly IUploadChatServices _uploadtService;

    public ChatController(IUploadChatServices uploadtService)
    {
        _uploadtService = uploadtService;
    }

    [HttpPost("send-message")]
    public async Task<IActionResult> SendMessage(MessageRequest messageRequest)
    {
        if (messageRequest == null || string.IsNullOrWhiteSpace(messageRequest.SenderId))
            return BadRequest(new { message = "Invalid chat request." });
        
        bool IsSent = await _uploadtService.UploadMessageAsync(messageRequest);
        return IsSent == true
            ? Ok("sent")
            : BadRequest("Sending failed");
    }

    [HttpPost("send-media")]
    public async Task<IActionResult> SendMedia(FileRequest fileRequest)
    {
        if(string.IsNullOrWhiteSpace(fileRequest.GroupId) || string.IsNullOrWhiteSpace(fileRequest.SenderId) || fileRequest.Files.Count == 0)
        {
            return BadRequest("value is null");
        }
        bool IsSent = await _uploadtService.UploadMediaAsync(fileRequest);
        return IsSent == true
            ? Ok("sent")
            : BadRequest("Sending failed");

    }

    
}