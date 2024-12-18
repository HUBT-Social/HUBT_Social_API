using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBT_Social_API.Src.Core.Helpers;
using HUBTSOCIAL.Src.Features.Chat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HUBT_Social_API.Features.Chat.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{

    private readonly IUploadChatServices _uploadtService;
    private readonly ITokenService _tokenService;

    public ChatController(IUploadChatServices uploadtService,ITokenService tokenService)
    {
        _uploadtService = uploadtService;
        _tokenService = tokenService;
    }

    [HttpPost("send-message")]
    public async Task<IActionResult> SendMessage(MessageInputRequest messageInputRequest)
    {
        if (messageInputRequest == null)
            return BadRequest(new { message = "Invalid chat request." });

        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (userResponse.Success == false)
        {
            BadRequest("Token is not validate");
        }
        
        MessageRequest messageRequest = (MessageRequest)messageInputRequest;
        messageRequest.SenderId = userResponse.User.Id.ToString();
        
        bool IsSent = await _uploadtService.UploadMessageAsync(messageRequest);
        return IsSent == true
            ? Ok("sent")
            : BadRequest("Sending failed");
    }

    [HttpPost("send-media")]
    public async Task<IActionResult> SendMedia(MediaInputRequest mediaInputRequest)
    {
        if(string.IsNullOrWhiteSpace(mediaInputRequest.GroupId)  || mediaInputRequest.Files.Count == 0)
        {
            return BadRequest("value is null");
        }

        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (userResponse.Success == false)
        {
            BadRequest("Token is not validate");
        }
        MediaRequest mediaRequest = (MediaRequest)mediaInputRequest;
        bool IsSent = await _uploadtService.UploadMediaAsync(mediaRequest);
        return IsSent == true
            ? Ok("sent")
            : BadRequest("Sending failed");

    }

    
}