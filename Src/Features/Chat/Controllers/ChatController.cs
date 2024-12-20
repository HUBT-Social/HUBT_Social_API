using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBT_Social_API.Src.Core.Helpers;
using HUBTSOCIAL.Src.Features.Chat.DTOs;
using HUBTSOCIAL.Src.Features.Chat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HUBT_Social_API.Features.Chat.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatRoomService;
    private readonly ITokenService _tokenService;
    public ChatController(IChatService chatRoomService,ITokenService tokenService)
    {
        _chatRoomService = chatRoomService;
        _tokenService = tokenService;
    }

    /// <summary>
    ///     Tạo nhóm chat mới.
    /// </summary>
    [HttpPost("create-group")]
    public async Task<IActionResult> CreateGroup(CreateGroupRequest createGroupRequest)
    {
        // Kiểm tra đầu vào
        var validationError = ValidateCreateGroupRequest(createGroupRequest);
        if (validationError != null)
            return BadRequest(new { message = validationError });

        // Lấy thông tin người dùng từ token
        UserResponse userResponse = await TokenHelper.GetUserResponseFromToken(Request, _tokenService);
        if (!userResponse.Success)
            return BadRequest(new { message = "Token is not valid" });

        // Tạo danh sách Participant
        var participants = CreateParticipants(createGroupRequest.UserNames, userResponse.User.UserName);

        // Tạo ChatRoomModel
        var newChatRoom = CreateChatRoom(createGroupRequest.GroupName, participants);

        // Lưu ChatRoom vào database
        var groupId = await _chatRoomService.CreateGroupAsync(newChatRoom);
        return groupId != null
            ? Ok(new { message = groupId })
            : BadRequest(new { message = LocalValue.Get(KeyStore.FailedToCreateGroup) });
    }
            // Phương thức kiểm tra đầu vào
            private string? ValidateCreateGroupRequest(CreateGroupRequest request)
            {
                if (string.IsNullOrEmpty(request.GroupName))
                    return LocalValue.Get(KeyStore.GroupNameRequired);
                if (request.UserNames.Count < 2)
                    return LocalValue.Get(KeyStore.NotEnoughMembers);
                return null;
            }
            // Phương thức tạo danh sách Participant
            private List<Participant> CreateParticipants(IEnumerable<string> userNames, string ownerUserName)
            {
                var participants = userNames
                    .Select(userName => new Participant 
                    { 
                        UserName = userName,
                        NickName = userName // Hiện tại là lấy theo userName, sau này sẽ lấy bằng cách call đến userService để lấy đúngđúng
                    })
                    .ToList();
                
                participants.Add(new Participant 
                { 
                    UserName = ownerUserName, 
                    Role = ParticipantRole.Owner,
                    NickName = ownerUserName
                });
                
                return participants;
            }
            // Phương thức tạo ChatRoomModel
            private ChatRoomModel CreateChatRoom(string groupName, List<Participant> participants)
            {
                return new ChatRoomModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = groupName,
                    AvatarUrl = LocalValue.Get(KeyStore.DefaultUserImage),
                    Participant = participants,
                    CreatedAt = DateTime.UtcNow
                };
            }
    
   


}