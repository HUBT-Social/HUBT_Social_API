using HUBT_Social_API.Core.Service.Upload;
using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBT_Social_API.Src.Core.Helpers;
using HUBTSOCIAL.Src.Features.Chat.Helpers;
using HUBTSOCIAL.Src.Features.Chat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Localization;

namespace HUBT_Social_API.Features.Chat.Controllers;

[ApiController]
[Route("api/chat/room")]
public class RoomController : ControllerBase
{

    private readonly ITokenService _tokenService;
    private readonly IRoomService _roomService;

    public RoomController(ITokenService tokenService,IRoomService roomService)
    {
        _tokenService = tokenService;
        _roomService = roomService;
    }


    [HttpGet("get-history-chat")]
    public async Task<IActionResult> GetHistoryChat([FromQuery] GetHistoryRequest getHistoryRequest)
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
            Types = new List<string> { "Message", "Media", "File"},
            Page = getHistoryRequest.Page,
            Limit = getHistoryRequest.Limit,
            Time = getHistoryRequest.Time
            
        };
        if (string.IsNullOrEmpty(getHistoryRequest.Time.ToString()))
        {
            getItemsHistoryRequest.Time = DateTime.Now;
        }
        IEnumerable<ChatItemResponse> chatItems = await _roomService.GetChatHistoryAsync(getItemsHistoryRequest);

        return Ok(chatItems);
    }
    [HttpGet("get-medias")]
    public async Task<IActionResult> GetMediasHistory([FromQuery] GetHistoryRequest getHistoryRequest)
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
            Types = new List<string> {"Media"},
            Page = getHistoryRequest.Page,
            Limit = getHistoryRequest.Limit,
            Time = getHistoryRequest.Time
        };
        if (string.IsNullOrEmpty(getHistoryRequest.Time.ToString()))
        {
            getItemsHistoryRequest.Time = DateTime.Now;
        }

        IEnumerable<ItemsHistoryRespone> chatItems = await _roomService.GetItemsHistoryAsync(getItemsHistoryRequest);

        return Ok(chatItems);
    }
    [HttpGet("get-files")]
    public async Task<IActionResult> GetFilesHistory([FromQuery] GetHistoryRequest getHistoryRequest)
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
            Types = new List<string> {"File"},
            Page = getHistoryRequest.Page,
            Limit = getHistoryRequest.Limit,
            Time = getHistoryRequest.Time
        };
        if (string.IsNullOrEmpty(getHistoryRequest.Time.ToString()))
        {
            getItemsHistoryRequest.Time = DateTime.Now;
        }

        IEnumerable<ItemsHistoryRespone> chatItems = await _roomService.GetItemsHistoryAsync(getItemsHistoryRequest);

        return Ok(chatItems);
    }
    [HttpGet("get-Links")]
    public async Task<IActionResult> GetLinksHistory([FromQuery] GetHistoryRequest getHistoryRequest)
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
            Types = new List<string> {"Link"},
            Page = getHistoryRequest.Page,
            Limit = getHistoryRequest.Limit,
            Time = getHistoryRequest.Time
        };
        if (string.IsNullOrEmpty(getHistoryRequest.Time.ToString()))
        {
            getItemsHistoryRequest.Time = DateTime.Now;
        }

        IEnumerable<ItemsHistoryRespone> chatItems = await _roomService.GetItemsHistoryAsync(getItemsHistoryRequest);

        return Ok(chatItems);
    }
     [HttpGet("get-voices")]
    public async Task<IActionResult> GetVoicesHistory([FromQuery] GetHistoryRequest getHistoryRequest)
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
            Types = new List<string> {"Voice"},
            Page = getHistoryRequest.Page,
            Limit = getHistoryRequest.Limit,
            Time = getHistoryRequest.Time
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
    public async Task<IActionResult> UpdateGroupName(UpdateGroupNameRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Id) || string.IsNullOrEmpty(request.NewName))
        {
            return BadRequest("Invalid request.");
        }
        // Lấy thông tin người dùng từ Token
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (userResponse == null || !userResponse.Success)
        {
            return BadRequest("Invalid or missing token.");
        }

        var result = await _roomService.UpdateGroupNameAsync(request.Id,userResponse.User.UserName,request.NewName);
        if (result)
        {
            return Ok("Group name updated successfully.");
        }
        return StatusCode(500, "Failed to update group name.");
    }

    // API to update avatar
    [HttpPut("update-group-avatar")]
    public async Task<IActionResult> UpdateAvatar(string GroupId,IFormFile file)
    {
        if (string.IsNullOrEmpty(GroupId) || file == null)
        {
            return BadRequest("Invalid request.");
        }
        // Lấy thông tin người dùng từ Token
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (userResponse == null || !userResponse.Success)
        {
            return BadRequest("Invalid or missing token.");
        }
        string? newUrl = await UploadToStoreS3.CloudinaryService.UploadToStorageAsync(file);

        var result = await _roomService.UpdateAvatarAsync(GroupId,userResponse.User.UserName,newUrl);
        if (result)
        {
            return Ok("Avatar updated successfully.");
        }
        return StatusCode(500, "Failed to update avatar.");
    }

    [HttpPut("update-nickname")]
    public async Task<IActionResult> UpdateNickNameAsync(UpdateNickNameRequest request)
    {
        if (request == null 
            || string.IsNullOrEmpty(request.GroupId) 
            || string.IsNullOrEmpty(request.UserName)
            || string.IsNullOrEmpty(request.NewNickName)
            )
        {
            return BadRequest("Invalid request.");
        }
        // Lấy thông tin người dùng từ Token
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (userResponse == null || !userResponse.Success)
        {
            return BadRequest("Invalid or missing token.");
        }

        var result = await _roomService.UpdateNickNameAsync(request.GroupId,userResponse.User.UserName, request.UserName, request.NewNickName);
        if (result)
            return Ok("Nickname updated successfully.");
        return BadRequest("Failed to update nickname.");
    }
    // API to add a member to the group
    [HttpPut("add-member")]
    public async Task<IActionResult> JoinRoomAsync(AddMemberRequest request)
    {
        if (request == null 
        || string.IsNullOrEmpty(request.GroupId) 
        || string.IsNullOrEmpty(request.AddedName)
        )
        {
            return BadRequest("Invalid request.");
        }
        // Lấy thông tin người dùng từ Token
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (userResponse == null || !userResponse.Success)
        {
            return BadRequest("Invalid or missing token.");
        }
        var result = await _roomService.JoinRoomAsync(request,userResponse.User.UserName);
        if (result)
        {   
            return Ok("Member added successfully.");
        }
        return StatusCode(500, "Failed to add member.");
    }

    // API to remove a member from the group
    [HttpPost("kick-member")]
    public async Task<IActionResult> KickMemberAsync(RemoveMemberRequest request)
    {
        // Kiểm tra đầu vào
        if (request == null 
        || string.IsNullOrEmpty(request.GroupId) 
        || string.IsNullOrEmpty(request.KickedName))
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
        ParticipantRole? kickerRole = await RoomChatHelper.GetRoleAsync(request.GroupId, userResponse.User.UserName);
        ParticipantRole? kickedRole = await RoomChatHelper.GetRoleAsync(request.GroupId, request.KickedName);

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
        bool result = await _roomService.KickMemberAsync(request,userResponse.User.UserName);
        if (result)
        {
            return Ok("Member removed successfully.");
        }

        // Xử lý khi xóa thất bại
        return StatusCode(500, "Failed to remove member.");
    }
    [HttpPost("leave-group")]
    public async Task<IActionResult> LeaveRoomAsync(LeaveRoomRequest request)
    {
        // Kiểm tra đầu vào
        if (request == null 
        || string.IsNullOrEmpty(request.GroupId))
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
        ParticipantRole? leaver = await RoomChatHelper.GetRoleAsync(request.GroupId, userResponse.User.UserName);
        

        // Kiểm tra quyền hạn của kicker
        if (leaver == null)
        {
            return BadRequest("Kicker or kicked user does not exist in the group.");
        }

        if (leaver == ParticipantRole.Owner)
        {
            return BadRequest("Let give your role to other member and try to exist after you have given role to someone.");
        }

        // Thực hiện hành động xóa thành viên
        bool result = await _roomService.LeaveRoomAsync(request.GroupId,userResponse.User.UserName);
        if (result)
        {
            return Ok("Member removed successfully.");
        }

        // Xử lý khi xóa thất bại
        return StatusCode(500, "Failed to remove member.");
    }
    
    [HttpPut("update-unsend-status")]
    public async Task<ActionResult> UpdateUnsendMessage(UpdateStatusMessageRequest unsendMessageRequest)
    {
        if(string.IsNullOrEmpty(unsendMessageRequest.ChatRoomId) 
        || string.IsNullOrEmpty(unsendMessageRequest.MessageId))
        {
            return BadRequest("Invalid request.");
        }
        // Lấy thông tin người dùng từ Token
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (userResponse == null || !userResponse.Success)
        {
            return BadRequest("Invalid or missing token.");
        }
        string? senderOfItem = await RoomChatHelper.GetInfoMessageAsync(unsendMessageRequest.ChatRoomId,unsendMessageRequest.MessageId);
        if(senderOfItem == null)
        {
            return BadRequest("Invalid value request.");
        }
        if(senderOfItem != userResponse.User.UserName)
        {
            return BadRequest("You are not owner of this message");
        }
        var updated = await _roomService.UpdateUnsendStatusAsync(unsendMessageRequest.ChatRoomId,unsendMessageRequest.MessageId);
        return updated ? Ok("Updated") : BadRequest("Err to Unsend.");
    }
    [HttpPut("update-pin-status")]
    public async Task<ActionResult> UpdatePinMessage(UpdateStatusMessageRequest unsendMessageRequest)
    {
        if(string.IsNullOrEmpty(unsendMessageRequest.ChatRoomId) 
        || string.IsNullOrEmpty(unsendMessageRequest.MessageId))
        {
            return BadRequest("Invalid request.");
        }

        var updated = await _roomService.UpdatePinStatusAsync(unsendMessageRequest.ChatRoomId,unsendMessageRequest.MessageId);
        return updated ? Ok("Updated") : BadRequest("Err to Pin.");
    }

}




