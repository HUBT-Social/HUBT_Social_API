using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HUBT_Social_API.Features.Chat.Controllers;

[ApiController]
[Route("api/chatroom")]
public class ChatRoomController : ControllerBase
{
    private readonly IChatRoomService _chatRoomService;

    public ChatRoomController(IChatRoomService chatRoomService)
    {
        _chatRoomService = chatRoomService;
    }

    /// <summary>
    ///     Tạo nhóm chat mới.
    /// </summary>
    [HttpPost("create-group")]
    public async Task<IActionResult> CreateGroup(CreateGroupRequest createGroupRequest)
    {
        if (string.IsNullOrEmpty(createGroupRequest.GroupName)) 
            return BadRequest(new { message = LocalValue.Get(KeyStore.GroupNameRequired) });
        if(createGroupRequest.UserIds.Count <2)
            return BadRequest(new {message = LocalValue.Get(KeyStore.NotEnoughMembers)});

        var groupId = await _chatRoomService.CreateGroupAsync(createGroupRequest);
        return groupId != null
            ? Ok(new {message = groupId})
            : BadRequest(new { message = LocalValue.Get(KeyStore.FailedToCreateGroup) });
    }


    // [HttpPut("{id}/update-name")]
    // public async Task<IActionResult> UpdateGroupName(string id, [FromBody] string newName)
    // {
        
    // }


    // [HttpDelete("{id}")]
    // public async Task<IActionResult> DeleteGroup(string id)
    // {
       
    // }

 
    // [HttpGet("{id}")]
    // public async Task<IActionResult> GetGroupById(string id)
    // {
        
    // }


    // [HttpGet]
    // public async Task<IActionResult> GetAllGroups()
    // {
        
    // }
}