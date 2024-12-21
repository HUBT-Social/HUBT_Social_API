using HUBT_Social_API.Features.Chat.Controllers;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBTSOCIAL.Src.Features.Chat.DTOs;
using HUBTSOCIAL.Src.Features.Chat.Models;
using static HUBT_Social_API.Features.Chat.Services.RoomService;

namespace HUBT_Social_API.Features.Chat.Services.Interfaces;

public interface IRoomService
{
    //Update methods
    // Update methods
    /// <summary>
    /// Cập nhật tên nhóm cho phòng chat.
    /// </summary>
    /// <param name="id">ID của phòng chat cần cập nhật.</param>
    /// <param name="newName">Tên nhóm mới.</param>
    /// <returns>Trả về true nếu cập nhật thành công, ngược lại false.</returns>
    Task<bool> UpdateGroupNameAsync(string id, string newName);

    /// <summary>
    /// Cập nhật URL avatar cho phòng chat.
    /// </summary>
    /// <param name="id">ID của phòng chat cần cập nhật.</param>
    /// <param name="newUrl">URL mới cho avatar.</param>
    /// <returns>Trả về true nếu cập nhật thành công, ngược lại false.</returns>
    Task<bool> UpdateAvatarAsync(string id, string newUrl);

    /// <summary>
    /// Cập nhật biệt danh (nickname) của một thành viên trong phòng chat.
    /// </summary>
    /// <param name="roomId">ID của phòng chat.</param>
    /// <param name="userName">Tên người dùng cần cập nhật biệt danh.</param>
    /// <param name="newNickName">Biệt danh mới.</param>
    /// <returns>Trả về true nếu cập nhật thành công, ngược lại false.</returns>
    Task<bool> UpdateNickNameAsync(string roomId, string userName, string newNickName);
    /// <summary>
    /// Cập nhật vai trò của một thành viên trong phòng chat.
    /// </summary>
    /// <param name="roomId">ID của phòng chat.</param>
    /// <param name="userName">Tên người dùng cần cập nhật vai trò.</param>
    /// <param name="newParticipantRole">Vai trò mới cần gán.</param>
    /// <returns>Trả về true nếu cập nhật thành công, ngược lại false.</returns>
    Task<bool> UpdateParticipantRole(string roomId, string userName,ParticipantRole newParticipantRole);
    /// <summary>
    /// Cập nhật trạng thái `Unsend` cho một `ChatItem`
    /// </summary>
    /// <param name="roomId">ID của phòng chat chứa `ChatItem`</param>
    /// <param name="chatItemId">ID của `ChatItem` cần cập nhật</param>
    /// <param name="unsend">Giá trị mới cho `Unsend`</param>
    /// <returns>Trả về `true` nếu cập nhật thành công, ngược lại `false`</returns>
    Task<bool> UpdateUnsendStatusAsync(string roomId, string chatItemId, bool unsend);

    /// <summary>
    /// Cập nhật trạng thái `IsPin` cho một `ChatItem`
    /// </summary>
    /// <param name="roomId">ID của phòng chat chứa `ChatItem`</param>
    /// <param name="chatItemId">ID của `ChatItem` cần cập nhật</param>
    /// <param name="isPin">Giá trị mới cho `IsPin`</param>
    /// <returns>Trả về `true` nếu cập nhật thành công, ngược lại `false`</returns>
    Task<bool> UpdatePinStatusAsync(string roomId, string chatItemId, bool isPin);
    /// <summary>
    /// Thêm một thành viên mới vào phòng chat.
    /// </summary>
    /// <param name="request">Yêu cầu thêm thành viên.</param>
    /// <returns>Trả về true nếu thêm thành công, ngược lại false.</returns>
    Task<bool> AddMemberAsync(AddMemberRequest request);
    /// <summary>
    /// Xóa một thành viên khỏi phòng chat.
    /// </summary>
    /// <param name="request">Yêu cầu xóa thành viên.</param>
    /// <returns>Trả về true nếu xóa thành công, ngược lại false.</returns>
    Task<bool> RemoveMemberAsync(RemoveMemberRequest request);
    /// <summary>
    /// Cho phép một thành viên rời khỏi phòng chat.
    /// </summary>
    /// <param name="GroupId">ID của phòng chat.</param>
    /// <param name="UserName">Tên người dùng muốn rời khỏi phòng.</param>
    /// <returns>Trả về true nếu rời phòng thành công, ngược lại false.</returns>
    Task<bool> LeaveRoomAsync(string GroupId, string UserName);
    Task<ParticipantRole?> GetRoleAsync(string roomId, string userName);
    Task<string?> GetNickNameAsync(string roomId, string userName);

    //Get methods
    Task<IEnumerable<ChatItemResponse>> GetChatHistoryAsync(GetItemsHistoryRequest getItemsHistoryRequest);
    Task<IEnumerable<ItemsHistoryRespone>> GetItemsHistoryAsync(GetItemsHistoryRequest getItemsHistoryRequest);
}