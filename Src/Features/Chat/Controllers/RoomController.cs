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
            UserName = userResponse.User.UserName.ToString()
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
            UserName = userResponse.User.UserName.ToString()
        };
        bool IsSent = await _uploadtService.UploadMediaAsync(mediaRequest);
        return IsSent == true
            ? Ok("sent")
            : BadRequest("Sending failed");

    }
    [HttpGet("get-history-chat")]
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
        IEnumerable<ChatItemResponse> chatItems = await _roomService.GetChatHistoryAsync(getItemsHistoryRequest);

        return Ok(chatItems);
    }
    [HttpGet("get-medias")]
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
    [HttpGet("get-files")]
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
    [HttpGet("get-Links")]
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
     [HttpGet("get-voices")]
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
    // API to update group name
    [HttpPut("update-group-name")]
    public async Task<IActionResult> UpdateGroupName([FromBody] UpdateGroupNameRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Id) || string.IsNullOrEmpty(request.NewName))
        {
            return BadRequest("Invalid request.");
        }

        var result = await _roomService.UpdateGroupNameAsync(request.Id,request.NewName);
        if (result)
        {
            return Ok("Group name updated successfully.");
        }
        return StatusCode(500, "Failed to update group name.");
    }

    // API to update avatar
    [HttpPut("update-avatar")]
    public async Task<IActionResult> UpdateAvatar([FromBody] UpdateAvatarRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Id) || request.file == null)
        {
            return BadRequest("Invalid request.");
        }
        string newUrl = await _uploadtService.UploadToStorageAsync(request.file);

        var result = await _roomService.UpdateAvatarAsync(request.Id,newUrl);
        if (result)
        {
            return Ok("Avatar updated successfully.");
        }
        return StatusCode(500, "Failed to update avatar.");
    }

    [HttpPut("update-nickname")]
    public async Task<IActionResult> UpdateNickNameAsync(UpdateNickNameRequest request)
    {
        var result = await _roomService.UpdateNickNameAsync(request.GroupId, request.UserName, request.NewNickName);
        if (result)
            return Ok("Nickname updated successfully.");
        return BadRequest("Failed to update nickname.");
    }
    // API to add a member to the group
    [HttpPost("add-member")]
    public async Task<IActionResult> AddMember([FromBody] AddMemberRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.GroupId) || string.IsNullOrEmpty(request.UserName))
        {
            return BadRequest("Invalid request.");
        }

        var result = await _roomService.AddMemberAsync(request);
        if (result)
        {   
            return Ok("Member added successfully.");
        }
        return StatusCode(500, "Failed to add member.");
    }

    // API to remove a member from the group
    [HttpDelete("remove-member")]
    public async Task<IActionResult> RemoveMember([FromBody] RemoveMemberRequest request)
    {
        // Kiểm tra đầu vào
        if (request == null || string.IsNullOrEmpty(request.GroupId) || string.IsNullOrEmpty(request.UserName))
        {
            return BadRequest("Invalid request.");
        }

        // Lấy thông tin người dùng từ Token
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (userResponse == null || !userResponse.Success)
        {
            return BadRequest("Invalid or missing token.");
        }

        // Lấy vai trò của người thực hiện hành động (kicker) và người bị kick (kicked)
        ParticipantRole? kickerRole = await _roomService.GetRoleAsync(request.GroupId, userResponse.User.UserName);
        ParticipantRole? kickedRole = await _roomService.GetRoleAsync(request.GroupId, request.UserName);

        // Kiểm tra quyền hạn của kicker
        if (kickerRole == null || kickedRole == null)
        {
            return BadRequest("Kicker or kicked user does not exist in the group.");
        }

        if (kickerRole <= kickedRole)
        {
            return BadRequest("You do not have sufficient permissions to remove this member.");
        }

        // Thực hiện hành động xóa thành viên
        bool result = await _roomService.RemoveMemberAsync(request);
        if (result)
        {
            return Ok("Member removed successfully.");
        }

        // Xử lý khi xóa thất bại
        return StatusCode(500, "Failed to remove member.");
    }
    [HttpPost("leave-room")]
    public async Task<ActionResult> LeaveRoom(LeaveRoomRequest leaveRoomRequest)
    {
        if(string.IsNullOrEmpty(leaveRoomRequest.GroupId))
        {
            return BadRequest("Invalid request.");
        }
        // Lấy thông tin người dùng từ Token
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (userResponse == null || !userResponse.Success)
        {
            return BadRequest("Invalid or missing token.");
        }
        bool leftSucceeded = await _roomService.LeaveRoomAsync(leaveRoomRequest.GroupId,userResponse.User.UserName);
        return leftSucceeded ? Ok("Left succeeded.") : BadRequest("Error to leave.");
    }

}




