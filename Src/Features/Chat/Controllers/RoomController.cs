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
[Route("api/chat/room")]
public class RoomController : ControllerBase
{

    private readonly IUploadChatServices _uploadtService;
    private readonly ITokenService _tokenService;
    private readonly IRoomService _roomService;

    public RoomController(IUploadChatServices uploadtService,ITokenService tokenService,IRoomService roomService)
    {
        _uploadtService = uploadtService;
        _tokenService = tokenService;
        _roomService = roomService;
    }

    [HttpPost("send-message")]
    public async Task<IActionResult> SendMessage(MessageInputRequest messageInputRequest)
    {
        if (messageInputRequest == null)
            return BadRequest(new { message = "Invalid chat request." });

        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (userResponse.Success == false)
        {
            return BadRequest("Token is not valid");
        }

        // Tạo một đối tượng MessageRequest từ MessageInputRequest
        MessageRequest messageRequest = new MessageRequest
        {
            GroupId = messageInputRequest.GroupId,
            Content = messageInputRequest.Content,
            SenderId = userResponse.User.Id.ToString()
        };

        bool isSent = await _uploadtService.UploadMessageAsync(messageRequest);

        return isSent
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
        // Tạo một đối tượng MediaRequest từ MediaInputRequest
        MediaRequest mediaRequest = new MediaRequest
        {
            GroupId = mediaInputRequest.GroupId,
            Files = mediaInputRequest.Files,
            SenderId = userResponse.User.Id.ToString()
        };
        bool IsSent = await _uploadtService.UploadMediaAsync(mediaRequest);
        return IsSent == true
            ? Ok("sent")
            : BadRequest("Sending failed");

    }
    [HttpPost("get-history-chat")]
    public async Task<IActionResult> GetHistoryChat([FromBody] GetHistoryRequest getHistoryRequest)
    {
        if (getHistoryRequest == null)
        {
            return BadRequest("Request body cannot be null");
        }

        if (string.IsNullOrWhiteSpace(getHistoryRequest.ChatRoomId))
        {
            return BadRequest("ChatRoomId cannot be null or empty");
        }
        
        GetItemsHistoryRequest getItemsHistoryRequest = new GetItemsHistoryRequest
        {
            ChatRoomId = getHistoryRequest.ChatRoomId,
            Types = new List<string> { "Message", "Media", "File"}
        };
        if (string.IsNullOrEmpty(getHistoryRequest.Time.ToString()))
        {
            getItemsHistoryRequest.Time = DateTime.Now;
        }
        IEnumerable<ChatHistoryResponse> chatItems = await _roomService.GetChatHistoryAsync(getItemsHistoryRequest);

        return Ok(chatItems);
    }
    [HttpPost("get-medias")]
    public async Task<IActionResult> GetMediasHistory([FromBody] GetHistoryRequest getHistoryRequest)
    {

        if (getHistoryRequest == null)
        {
            return BadRequest("Request body cannot be null");
        }

        if (string.IsNullOrWhiteSpace(getHistoryRequest.ChatRoomId))
        {
            return BadRequest("ChatRoomId cannot be null or empty");
        }

        GetItemsHistoryRequest getItemsHistoryRequest = new GetItemsHistoryRequest
        {
            ChatRoomId = getHistoryRequest.ChatRoomId,
            Types = new List<string> {"Media"}
        };
        if (string.IsNullOrEmpty(getHistoryRequest.Time.ToString()))
        {
            getItemsHistoryRequest.Time = DateTime.Now;
        }

        IEnumerable<ItemsHistoryRespone> chatItems = await _roomService.GetItemsHistoryAsync(getItemsHistoryRequest);

        return Ok(chatItems);
    }
    [HttpPost("get-files")]
    public async Task<IActionResult> GetFilesHistory([FromBody] GetHistoryRequest getHistoryRequest)
    {

        if (getHistoryRequest == null)
        {
            return BadRequest("Request body cannot be null");
        }

        if (string.IsNullOrWhiteSpace(getHistoryRequest.ChatRoomId))
        {
            return BadRequest("ChatRoomId cannot be null or empty");
        }

        GetItemsHistoryRequest getItemsHistoryRequest = new GetItemsHistoryRequest
        {
            ChatRoomId = getHistoryRequest.ChatRoomId,
            Types = new List<string> {"File"}
        };
        if (string.IsNullOrEmpty(getHistoryRequest.Time.ToString()))
        {
            getItemsHistoryRequest.Time = DateTime.Now;
        }

        IEnumerable<ItemsHistoryRespone> chatItems = await _roomService.GetItemsHistoryAsync(getItemsHistoryRequest);

        return Ok(chatItems);
    }
    [HttpPost("get-Links")]
    public async Task<IActionResult> GetLinksHistory([FromBody] GetHistoryRequest getHistoryRequest)
    {

        if (getHistoryRequest == null)
        {
            return BadRequest("Request body cannot be null");
        }

        if (string.IsNullOrWhiteSpace(getHistoryRequest.ChatRoomId))
        {
            return BadRequest("ChatRoomId cannot be null or empty");
        }

        GetItemsHistoryRequest getItemsHistoryRequest = new GetItemsHistoryRequest
        {
            ChatRoomId = getHistoryRequest.ChatRoomId,
            Types = new List<string> {"Link"}
        };
        if (string.IsNullOrEmpty(getHistoryRequest.Time.ToString()))
        {
            getItemsHistoryRequest.Time = DateTime.Now;
        }

        IEnumerable<ItemsHistoryRespone> chatItems = await _roomService.GetItemsHistoryAsync(getItemsHistoryRequest);

        return Ok(chatItems);
    }
     [HttpPost("get-voices")]
    public async Task<IActionResult> GetVoicesHistory([FromBody] GetHistoryRequest getHistoryRequest)
    {

        if (getHistoryRequest == null)
        {
            return BadRequest("Request body cannot be null");
        }

        if (string.IsNullOrWhiteSpace(getHistoryRequest.ChatRoomId))
        {
            return BadRequest("ChatRoomId cannot be null or empty");
        }

        GetItemsHistoryRequest getItemsHistoryRequest = new GetItemsHistoryRequest
        {
            ChatRoomId = getHistoryRequest.ChatRoomId,
            Types = new List<string> {"Voice"}
        };
        if (string.IsNullOrEmpty(getHistoryRequest.Time.ToString()))
        {
            getItemsHistoryRequest.Time = DateTime.Now;
        }

        IEnumerable<ItemsHistoryRespone> chatItems = await _roomService.GetItemsHistoryAsync(getItemsHistoryRequest);

        return Ok(chatItems);
    }
    
}