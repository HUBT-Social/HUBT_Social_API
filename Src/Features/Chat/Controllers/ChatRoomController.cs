using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
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
    public async Task<IActionResult> CreateGroup([FromBody] string nameGroup)
    {
        if (string.IsNullOrEmpty(nameGroup)) return BadRequest(new { message = LocalValue.Get(KeyStore.GroupNameRequired) });

        var result = await _chatRoomService.CreateGroupAsync(nameGroup);
        return result
            ? Ok(new { message = LocalValue.Get(KeyStore.GroupCreatedSuccessfully) })
            : BadRequest(new { message = LocalValue.Get(KeyStore.FailedToCreateGroup) });
    }

    /// <summary>
    ///     Cập nhật tên nhóm chat.
    /// </summary>
    [HttpPut("{id}/update-name")]
    public async Task<IActionResult> UpdateGroupName(string id, [FromBody] string newName)
    {
        if (string.IsNullOrEmpty(newName)) return BadRequest(new { message = LocalValue.Get(KeyStore.GroupNameRequired) });

        var result = await _chatRoomService.UpdateGroupNameAsync(id, newName);
        return result
            ? Ok(new { message = LocalValue.Get(KeyStore.GroupNameUpdatedSuccessfully) })
            : BadRequest(new { message = LocalValue.Get(KeyStore.FailedToUpdateGroup) });
    }

    /// <summary>
    ///     Xóa nhóm chat.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGroup(string id)
    {
        var result = await _chatRoomService.DeleteGroupAsync(id);
        return result
            ? Ok(new { message = LocalValue.Get(KeyStore.GroupDeletedSuccessfully) })
            : BadRequest(new { message = LocalValue.Get(KeyStore.FailedToDeleteGroup) });
    }

    /// <summary>
    ///     Lấy thông tin nhóm chat theo ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetGroupById(string id)
    {
        var chatRoom = await _chatRoomService.GetGroupByIdAsync(id);
        return chatRoom != null
            ? Ok(chatRoom)
            : NotFound(new { message = LocalValue.Get(KeyStore.GroupNotFound) });
    }

    /// <summary>
    ///     Lấy tất cả nhóm chat.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllGroups()
    {
        var chatRooms = await _chatRoomService.GetAllGroupsAsync();
        return Ok(chatRooms);
    }
}