

using System.ComponentModel;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.Features.Chat.ChatHubs;
using HUBT_Social_API.Features.Chat.Controllers;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.Helpers;
using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace HUBT_Social_API.Features.Chat.Services;

public class RoomService : IRoomService
{

    private readonly IMongoCollection<ChatRoomModel> _chatRooms;
    private readonly IUserService _userService;
    private readonly ChatHub _chatHub;

    public RoomService(IMongoCollection<ChatRoomModel> chatRooms,IUserService userService,ChatHub chatHub)
    {
        _chatRooms = chatRooms;
        _userService = userService;
        _chatHub = chatHub;
    }
// Update methods
    /// <summary>
    /// Cập nhật tên nhóm cho phòng chat.
    /// </summary>
    /// <param name="id">ID của phòng chat cần cập nhật.</param>
    /// <param name="newName">Tên nhóm mới.</param>
    /// <returns>Trả về true nếu cập nhật thành công, ngược lại false.</returns>
    public async Task<bool> UpdateGroupNameAsync(string id, string newName)
    {
        try
        {
            // Tạo định nghĩa cập nhật cho trường Name
            UpdateDefinition<ChatRoomModel> update = Builders<ChatRoomModel>.Update.Set(c => c.Name, newName); 

            // Thực hiện cập nhật phòng chat dựa trên ID
            UpdateResult result = await _chatRooms.UpdateOneAsync(c => c.Id == id, update);

            // Trả về true nếu số lượng bản ghi được cập nhật lớn hơn 0
            return result.ModifiedCount > 0; 
        }
        catch
        {
            // Trả về false nếu xảy ra lỗi
            return false;
        }
    }

    /// <summary>
    /// Cập nhật URL avatar cho phòng chat.
    /// </summary>
    /// <param name="id">ID của phòng chat cần cập nhật.</param>
    /// <param name="newUrl">URL mới cho avatar.</param>
    /// <returns>Trả về true nếu cập nhật thành công, ngược lại false.</returns>
    public async Task<bool> UpdateAvatarAsync(string id, string newUrl)
    {
        try
        {
            // Tạo định nghĩa cập nhật cho trường AvatarUrl
            UpdateDefinition<ChatRoomModel> update = Builders<ChatRoomModel>.Update.Set(c => c.AvatarUrl, newUrl); 

            // Thực hiện cập nhật phòng chat dựa trên ID
            UpdateResult result = await _chatRooms.UpdateOneAsync(c => c.Id == id, update);

            // Trả về true nếu số lượng bản ghi được cập nhật lớn hơn 0
            return result.ModifiedCount > 0;
        }
        catch
        {
            // Trả về false nếu xảy ra lỗi
            return false;
        }
    }

    /// <summary>
    /// Cập nhật biệt danh (nickname) của một thành viên trong phòng chat.
    /// </summary>
    /// <param name="roomId">ID của phòng chat.</param>
    /// <param name="userName">Tên người dùng cần cập nhật biệt danh.</param>
    /// <param name="newNickName">Biệt danh mới.</param>
    /// <returns>Trả về true nếu cập nhật thành công, ngược lại false.</returns>
    public async Task<bool> UpdateNickNameAsync(string roomId, string userName, string newNickName)
    {
        try
        {
            // Tạo bộ lọc tìm phòng chat với `roomId` và `Participant` có `UserName` khớp
            var filter = Builders<ChatRoomModel>.Filter.And(
                Builders<ChatRoomModel>.Filter.Eq(r => r.Id, roomId),
                Builders<ChatRoomModel>.Filter.ElemMatch(r => r.Participant, p => p.UserName == userName)
            );

            // Tạo định nghĩa cập nhật để thay đổi `NickName` của `Participant` khớp
            var update = Builders<ChatRoomModel>.Update.Set(
                r => r.Participant[-1].NickName, // `-1` đại diện cho phần tử được tìm qua `ElemMatch`
                newNickName
            );

            // Thực hiện cập nhật
            var result = await _chatRooms.UpdateOneAsync(filter, update);

            // Kiểm tra số lượng bản ghi bị thay đổi
            return result.ModifiedCount > 0;
        }
        catch
        {
            // Trả về false nếu xảy ra lỗi
            return false;
        }
    }
    /// <summary>
    /// Cập nhật vai trò của một thành viên trong phòng chat.
    /// </summary>
    /// <param name="roomId">ID của phòng chat.</param>
    /// <param name="userName">Tên người dùng cần cập nhật vai trò.</param>
    /// <param name="newParticipantRole">Vai trò mới cần gán.</param>
    /// <returns>Trả về true nếu cập nhật thành công, ngược lại false.</returns>
    public async Task<bool> UpdateParticipantRole(string roomId, string userName,ParticipantRole newParticipantRole)
    {
        try
        {
            // Tìm phòng chat chứa `roomId` và kiểm tra `Participant` có `UserName` khớp
            var filter = Builders<ChatRoomModel>.Filter.And(
                Builders<ChatRoomModel>.Filter.Eq(r => r.Id, roomId),
                Builders<ChatRoomModel>.Filter.ElemMatch(r => r.Participant, p => p.UserName == userName)
            );

            // Xác định phần tử cụ thể trong mảng `Participant` để cập nhật `NickName`
            var update = Builders<ChatRoomModel>.Update.Set(
                r => r.Participant[-1].Role, newParticipantRole // -1 đại diện cho phần tử được tìm qua `ElemMatch`
            );

            // Thực hiện cập nhật
            var result = await _chatRooms.UpdateOneAsync(filter, update);

            // Kiểm tra số lượng bản ghi bị thay đổi
            return result.ModifiedCount > 0;
        }
        catch
        {
            return false;
        }
    }
    /// <summary>
    /// Cập nhật trạng thái `Unsend` cho một `ChatItem`
    /// </summary>
    /// <param name="roomId">ID của phòng chat chứa `ChatItem`</param>
    /// <param name="chatItemId">ID của `ChatItem` cần cập nhật</param>
    /// <param name="unsend">Giá trị mới cho `Unsend`</param>
    /// <returns>Trả về `true` nếu cập nhật thành công, ngược lại `false`</returns>
    public async Task<bool> UpdateUnsendStatusAsync(string roomId, string chatItemId, bool unsend)
    {
        try
        {
            var filter = Builders<ChatRoomModel>.Filter.And(
                Builders<ChatRoomModel>.Filter.Eq(room => room.Id, roomId),
                Builders<ChatRoomModel>.Filter.ElemMatch(
                    room => room.ChatItems,
                    item => item.Id == chatItemId
                )
            );

            var update = Builders<ChatRoomModel>.Update.Set("ChatItems.$.Unsend", unsend);

            var result = await _chatRooms.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Cập nhật trạng thái `IsPin` cho một `ChatItem`
    /// </summary>
    /// <param name="roomId">ID của phòng chat chứa `ChatItem`</param>
    /// <param name="chatItemId">ID của `ChatItem` cần cập nhật</param>
    /// <param name="isPin">Giá trị mới cho `IsPin`</param>
    /// <returns>Trả về `true` nếu cập nhật thành công, ngược lại `false`</returns>
    public async Task<bool> UpdatePinStatusAsync(string roomId, string chatItemId, bool isPin)
    {
        try
        {
            var filter = Builders<ChatRoomModel>.Filter.And(
                Builders<ChatRoomModel>.Filter.Eq(room => room.Id, roomId),
                Builders<ChatRoomModel>.Filter.ElemMatch(
                    room => room.ChatItems,
                    item => item.Id == chatItemId
                )
            );

            var update = Builders<ChatRoomModel>.Update.Set("ChatItems.$.IsPin", isPin);

            var result = await _chatRooms.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Thêm một thành viên mới vào phòng chat.
    /// </summary>
    /// <param name="request">Yêu cầu thêm thành viên.</param>
    /// <returns>Trả về true nếu thêm thành công, ngược lại false.</returns>
    public async Task<bool> AddMemberAsync(AddMemberRequest request)
    {
        try
        {
            var filter = Builders<ChatRoomModel>.Filter.Eq(r => r.Id, request.GroupId);
            var update = Builders<ChatRoomModel>.Update.AddToSet(r => r.Participant, new Participant
            {
                UserName = request.UserName
            });

            var result = await _chatRooms.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;  
        }catch
        {
            return false;
        }
        
    }
    /// <summary>
    /// Xóa một thành viên khỏi phòng chat.
    /// </summary>
    /// <param name="request">Yêu cầu xóa thành viên.</param>
    /// <returns>Trả về true nếu xóa thành công, ngược lại false.</returns>
    public async Task<bool> RemoveMemberAsync(RemoveMemberRequest request)
    {
        try
        {
            var filter = Builders<ChatRoomModel>.Filter.Eq(r => r.Id, request.GroupId);
            var update = Builders<ChatRoomModel>.Update.PullFilter(r => r.Participant,
                p => p.UserName == request.UserName);

            var result = await _chatRooms.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
        catch
        {
            return false;
        }
        
    }
    /// <summary>
    /// Cho phép một thành viên rời khỏi phòng chat.
    /// </summary>
    /// <param name="GroupId">ID của phòng chat.</param>
    /// <param name="UserName">Tên người dùng muốn rời khỏi phòng.</param>
    /// <returns>Trả về true nếu rời phòng thành công, ngược lại false.</returns>
    public async Task<bool> LeaveRoomAsync(string GroupId, string UserName)
    {
        try
        {
            // Tạo bộ lọc để tìm phòng chat dựa trên GroupId
            var filter = Builders<ChatRoomModel>.Filter.Eq(r => r.Id, GroupId);

            // Tạo bản cập nhật để loại bỏ UserName khỏi danh sách Members
            var update = Builders<ChatRoomModel>.Update.PullFilter(
                r => r.Participant,
                p => p.UserName == UserName
            );

            // Thực hiện cập nhật
            var result = await _chatRooms.UpdateOneAsync(filter, update);

            // Kiểm tra kết quả
            return result.ModifiedCount > 0;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> UnsendMessageAsync(string GroupId, string MessageId)
    {
        try
        {
            var filter = Builders<ChatRoomModel>.Filter.Eq(cr => cr.Id, GroupId);
            ChatRoomModel? chatRoom = await _chatRooms.Find(filter).FirstOrDefaultAsync();
            if(chatRoom == null)
            {
                return false;
            }
            var message = chatRoom.ChatItems.FirstOrDefault(ci => ci.Id == MessageId);
            if (message == null)
            {
                return false;
            }
            // Cập nhật trạng thái Unsend
            message.Unsend = true;

            // Lưu thay đổi vào cơ sở dữ liệu
            var update = Builders<ChatRoomModel>.Update.Set(cr => cr.ChatItems, chatRoom.ChatItems);
            await _chatRooms.UpdateOneAsync(filter, update);

            // Gửi sự kiện đến tất cả người dùng trong phòng
            await _chatHub.UnSendChatItem(GroupId, MessageId);

            return true;
        }
        catch
        {
            return false;
        }
        
    }
    //Get methods
    public async Task<IEnumerable<ChatItemResponse>> GetChatHistoryAsync(GetItemsHistoryRequest getItemsHistoryRequest)
    {
        // Lấy danh sách các ChatItems từ phương thức GetItemsAsync
        var chatItems = await GetItemsAsync(getItemsHistoryRequest);

        // Duyệt qua từng ChatItem để tạo danh sách ChatItemResponse
        var response = new List<ChatItemResponse>();
        foreach (var item in chatItems)
        {
            var chatItemResponse = new ChatItemResponse
            {
                Id = item.Id,
                NickName = await RoomChatHelper.GetNickNameAsync(getItemsHistoryRequest.ChatRoomId, item.UserName) ?? item.UserName,
                AvatarUrl = await _userService.GetAvatarUrlFromUserName(item.UserName),
                Timestamp = item.Timestamp,
                Type = item.Type,
                Data = item.ToResponseData()
            };
            response.Add(chatItemResponse);
        }

        return response;
    }
    public async Task<IEnumerable<ItemsHistoryRespone>> GetItemsHistoryAsync(GetItemsHistoryRequest getItemsHistoryRequest)
    {
        var chatItems = await GetItemsAsync(getItemsHistoryRequest);

        var response = chatItems.Select(item => new ItemsHistoryRespone
        {
            Data = item.ToResponseData()
        }).ToList();

        return response;
    }
        private async Task<List<ChatItem>> GetItemsAsync(GetItemsHistoryRequest getItemsHistoryRequest)
        {
            // Giả lập dữ liệu từ `ChatRoomModel` (thay thế bằng kết nối DB thực tế)
            var chatRoom = _chatRooms.Find(room => room.Id == getItemsHistoryRequest.ChatRoomId).FirstOrDefault();

            if (chatRoom == null)
                return new List<ChatItem>();

            // Lọc các item theo thời gian và loại (nếu có)
            var filteredItems = chatRoom.ChatItems
                .Where(item =>
                    (getItemsHistoryRequest.Time == null || item.Timestamp < getItemsHistoryRequest.Time) &&
                    (getItemsHistoryRequest.Types == null || getItemsHistoryRequest.Types.Contains(item.Type)))
                .OrderByDescending(item => item.Timestamp)
                .Take(20)
                .ToList();

            return filteredItems;
        }


    

}