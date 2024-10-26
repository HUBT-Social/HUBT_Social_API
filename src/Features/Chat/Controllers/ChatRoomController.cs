using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;
using System.Collections.Generic;
using HUBTSOCIAL.Src.Features.Chat.Services;
using HUBTSOCIAL.Src.Features.Chat.Models;
using HUBTSOCIAL.Src.Features.Chat.DTOs;
using HUBTSOCIAL.Src.Features.Chat.Services.IChatServices;

namespace HUBTSOCIAL.Src.Features.Chat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatRoomController : ControllerBase
    {
        private readonly IChatRoomService _chatRoomService;
        private readonly IStringLocalizer<ChatRoomController> _localizer;

        public ChatRoomController(IChatRoomService chatRoomService, IStringLocalizer<ChatRoomController> localizer)
        {
            _chatRoomService = chatRoomService;
            _localizer = localizer;
        }

        /// <summary>
        /// Tạo nhóm chat mới.
        /// </summary>
        [HttpPost("create-group")]
        public async Task<IActionResult> CreateGroup([FromBody] string nameGroup)
        {
            if (string.IsNullOrEmpty(nameGroup))
            {
                return BadRequest(new { message = _localizer["GroupNameRequired"] });
            }

            bool result = await _chatRoomService.CreateGroupAsync(nameGroup);
            return result
                ? Ok(new { message = _localizer["GroupCreatedSuccessfully"] })
                : BadRequest(new { message = _localizer["FailedToCreateGroup"] });
        }

        /// <summary>
        /// Cập nhật tên nhóm chat.
        /// </summary>
        [HttpPut("{id}/update-name")]
        public async Task<IActionResult> UpdateGroupName(string id, [FromBody] string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                return BadRequest(new { message = _localizer["GroupNameRequired"] });
            }

            bool result = await _chatRoomService.UpdateGroupNameAsync(id, newName);
            return result
                ? Ok(new { message = _localizer["GroupNameUpdatedSuccessfully"] })
                : BadRequest(new { message = _localizer["FailedToUpdateGroup"] });
        }

        /// <summary>
        /// Xóa nhóm chat.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(string id)
        {
            bool result = await _chatRoomService.DeleteGroupAsync(id);
            return result
                ? Ok(new { message = _localizer["GroupDeletedSuccessfully"] })
                : BadRequest(new { message = _localizer["FailedToDeleteGroup"] });
        }

        /// <summary>
        /// Lấy thông tin nhóm chat theo ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroupById(string id)
        {
            var chatRoom = await _chatRoomService.GetGroupByIdAsync(id);
            return chatRoom != null
                ? Ok(chatRoom)
                : NotFound(new { message = _localizer["GroupNotFound"] });
        }

        /// <summary>
        /// Lấy tất cả nhóm chat.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllGroups()
        {
            var chatRooms = await _chatRoomService.GetAllGroupsAsync();
            return Ok(chatRooms);
        }
    }
}
