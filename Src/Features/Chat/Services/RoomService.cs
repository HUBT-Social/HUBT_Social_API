

using System.ComponentModel;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.Features.Chat.ChatHubs;
using HUBT_Social_API.Features.Chat.Controllers;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.Collections;
using HUBTSOCIAL.Src.Features.Chat.Helpers;
using HUBTSOCIAL.Src.Features.Chat.Models;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace HUBT_Social_API.Features.Chat.Services;

public class RoomService : IRoomService
{

    private readonly IMongoCollection<ChatRoomModel> _chatRooms;
    private readonly IUserService _userService;
    public readonly IHubContext<ChatHub> _hubContext;
    private readonly IUserConnectionManager _userConnectionManager;

    public RoomService(
        IMongoCollection<ChatRoomModel> chatRooms,
        IUserService userService,IHubContext<ChatHub> hubContext,
        IUserConnectionManager userConnectionManager)
    {
        _chatRooms = chatRooms;
        _userService = userService;
        _hubContext = hubContext;
        _userConnectionManager = userConnectionManager;
    }
// Update methods
    /// <summary>
    /// Cập nhật tên nhóm cho phòng chat.
    /// </summary>
    /// <param name="id">ID của phòng chat cần cập nhật.</param>
    /// <param name="newName">Tên nhóm mới.</param>
    /// <returns>Trả về true nếu cập nhật thành công, ngược lại false.</returns>
    public async Task<bool> UpdateGroupNameAsync(string id, string userName, string newName)
    {
        try
        {
            // Kiểm tra xem phòng chat có tồn tại không
            var chatRoom = await _chatRooms.Find(c => c.Id == id).FirstOrDefaultAsync();
            if (chatRoom == null)
            {
                Console.WriteLine($"Chat room with ID {id} does not exist.");
                return false;
            }

            // Tạo định nghĩa cập nhật cho trường Name
            UpdateDefinition<ChatRoomModel> update = Builders<ChatRoomModel>.Update.Set(c => c.Name, newName);

            // Thực hiện cập nhật phòng chat dựa trên ID
            UpdateResult result = await _chatRooms.UpdateOneAsync(c => c.Id == id, update);

            if (result.ModifiedCount > 0)
            {
                // Gửi thông báo qua SignalR nếu cập nhật thành công
                await _hubContext.Clients.Group(id).SendAsync("UpdateGroupName",  new {userName = userName, newName = newName});
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating group name: {ex.Message}");
            return false;
        }
    }



    /// <summary>
    /// Cập nhật URL avatar cho phòng chat.
    /// </summary>
    /// <param name="id">ID của phòng chat cần cập nhật.</param>
    /// <param name="newUrl">URL mới cho avatar.</param>
    /// <returns>Trả về true nếu cập nhật thành công, ngược lại false.</returns>
    public async Task<bool> UpdateAvatarAsync(string id, string userName, string newUrl)
    {
        try
        {
            // Kiểm tra đầu vào
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(newUrl))
            {
                return false;
            }

            // Kiểm tra xem phòng chat có tồn tại không
            var chatRoom = await _chatRooms.Find(c => c.Id == id).FirstOrDefaultAsync();
            if (chatRoom == null)
            {
                return false;
            }

            // Tạo định nghĩa cập nhật cho trường AvatarUrl
            UpdateDefinition<ChatRoomModel> update = Builders<ChatRoomModel>.Update.Set(c => c.AvatarUrl, newUrl);

            // Thực hiện cập nhật phòng chat dựa trên ID
            UpdateResult result = await _chatRooms.UpdateOneAsync(c => c.Id == id, update);


            // Trả về true nếu số lượng bản ghi được cập nhật lớn hơn 0
            if(result.ModifiedCount > 0)
            {
                // Gửi thông báo qua SignalR nếu cập nhật thành công
                await _hubContext.Clients.Group(id).SendAsync("UpdateAvatar", new {userName = userName, newUrl = newUrl});
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            // Ghi log lỗi
            Console.WriteLine($"Error in UpdateAvatarAsync: {ex.Message}");
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
    public async Task<bool> UpdateNickNameAsync(string roomId,string changerName ,string userName, string newNickName)
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
            // Trả về true nếu số lượng bản ghi được cập nhật lớn hơn 0
            if(result.ModifiedCount > 0)
            {
                // Gửi thông báo qua SignalR nếu cập nhật thành công
                await _hubContext.Clients.Group(roomId).SendAsync("UpdateNickName", new {changerName=changerName,changed = userName, nickName = newNickName});
                return true;
            }
            return false;
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
    public async Task<bool> UpdateParticipantRoleAsync(string roomId, string userName, ParticipantRole newParticipantRole)
{
    try
    {
        // Tìm phòng chat chứa `roomId` và `Participant` có `UserName` khớp
        var filter = Builders<ChatRoomModel>.Filter.And(
            Builders<ChatRoomModel>.Filter.Eq(r => r.Id, roomId),
            Builders<ChatRoomModel>.Filter.ElemMatch(r => r.Participant, p => p.UserName == userName)
        );

        // Kiểm tra sự tồn tại trước khi cập nhật
        var chatRoom = await _chatRooms.Find(filter).FirstOrDefaultAsync();
        if (chatRoom == null)
        {
            return false;
        }

        // Cập nhật trường `Role` của phần tử tìm được trong mảng `Participant`
        var update = Builders<ChatRoomModel>.Update.Set("Participant.$.Role", newParticipantRole);

        // Thực hiện cập nhật
        var result = await _chatRooms.UpdateOneAsync(filter, update);

        // Trả về true nếu số lượng bản ghi được cập nhật lớn hơn 0
            if(result.ModifiedCount > 0)
            {
                // Gửi thông báo qua SignalR nếu cập nhật thành công
                await _hubContext.Clients.Group(roomId).SendAsync("UpdateParticipantRole", new {userName = userName, newRole = newParticipantRole});
                return true;
            }
            return false;
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
    public async Task<bool> UpdateActionStatusAsync(string roomId, string chatItemId,MessageActionStatus newActionStatus)
    {
        try
        {
            var filter = Builders<ChatRoomModel>.Filter.And(
                Builders<ChatRoomModel>.Filter.Eq(room => room.Id, roomId),
                Builders<ChatRoomModel>.Filter.ElemMatch(
                    room => room.Content,
                    item => item.id == chatItemId
                )
            );
            // Lấy mục chat hiện tại để kiểm tra trạng thái Unsend
            var chatRoom = await _chatRooms.Find(filter).FirstOrDefaultAsync();
            if (chatRoom == null)
            {
                return false; // Không tìm thấy phòng hoặc mục chat
            }

            // Tìm mục chat tương ứng và lấy trạng thái Unsend hiện tại
            var chatItem = chatRoom.Content.FirstOrDefault(item => item.id == chatItemId);
            if (chatItem == null)
            {
                return false; // Không tìm thấy mục chat
            }

            // Cập nhật trạng thái Unsend mới
            var update = Builders<ChatRoomModel>.Update.Set(
                "Content.$.ActionStatus"
                , newActionStatus
            );

            var result = await _chatRooms.UpdateOneAsync(filter, update);

            if(result.ModifiedCount > 0)
            {
                await _hubContext.Clients.Group(roomId).SendAsync("UpdateActionStatus", new {chatItemId = chatItemId, status = newActionStatus});
                return true;
            }
            return false;
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
    public async Task<bool> JoinRoomAsync(AddMemberRequest request,string AdderName)
    {
        try
        {
            var filter = Builders<ChatRoomModel>.Filter.Eq(r => r.Id, request.GroupId);
            string Adder = await _userService.GetFullName(AdderName) ?? AdderName;
            string Added = await _userService.GetFullName(request.AddedName) ?? request.AddedName;
            var update = Builders<ChatRoomModel>.Update.AddToSet(r => r.Participant, new Participant()
            {
                UserName = request.AddedName,
                NickName = Added,
            });

            var result = await _chatRooms.UpdateOneAsync(filter, update);
            if(result.ModifiedCount > 0)
            {
                var connectionId = _userConnectionManager.GetConnectionId(request.AddedName);
                if (connectionId != null)
                {
                    await _hubContext.Groups.AddToGroupAsync(connectionId, request.GroupId);
                }
                // Gửi thông báo qua SignalR nếu cập nhật thành công
                await _hubContext.Clients.Group(request.GroupId).SendAsync("AddMember", new {Adder = Adder,Added = Added});
                return true;
            }
            return false;  
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
    public async Task<bool> KickMemberAsync(RemoveMemberRequest request,string KickerName)
    {
        try
        {
            var filter = Builders<ChatRoomModel>.Filter.Eq(r => r.Id, request.GroupId);

            string Kicker = await _userService.GetFullName(KickerName) ?? KickerName;
            string Kicked = await _userService.GetFullName(request.KickedName) ?? request.KickedName;

            var update = Builders<ChatRoomModel>.Update.PullFilter(r => r.Participant,
                p => p.UserName == request.KickedName);

            var result = await _chatRooms.UpdateOneAsync(filter, update);
             if(result.ModifiedCount > 0)
            {
                // Gửi thông báo qua SignalR nếu cập nhật thành công
                var connectionId = _userConnectionManager.GetConnectionId(request.KickedName);
                if (connectionId != null)
                {
                    await _hubContext.Groups.RemoveFromGroupAsync(connectionId, request.GroupId);
                }
                await _hubContext.Clients.Group(request.GroupId).SendAsync("KickMember", new {Kicker = Kicker,Kicked = Kicked});
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
        
    }
    public async Task<bool> LeaveRoomAsync(string groupId,string userName)
    {
        try
        {
            var filter = Builders<ChatRoomModel>.Filter.Eq(r => r.Id, groupId);

            string NickName = await _userService.GetFullName(userName) ?? userName;

            var update = Builders<ChatRoomModel>.Update.PullFilter(r => r.Participant,
                p => p.UserName == userName);

            var result = await _chatRooms.UpdateOneAsync(filter, update);
             if(result.ModifiedCount > 0)
            {
                // Gửi thông báo qua SignalR nếu cập nhật thành công
                var connectionId = _userConnectionManager.GetConnectionId(userName);
                if (connectionId != null)
                {
                    await _hubContext.Groups.RemoveFromGroupAsync(connectionId, groupId);
                }
                await _hubContext.Clients.Group(groupId).SendAsync("LeaveRoom", userName);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
        
    }
 

    
    //Get methods
    // public async Task<IEnumerable<ChatItemResponse>> GetChatHistoryAsync(GetItemsHistoryRequest getItemsHistoryRequest)
    // {
    //     // Lấy danh sách các ChatItems từ phương thức GetItemsAsync
    //     var chatItems = await GetItemsAsync(getItemsHistoryRequest);

    //     // Duyệt qua từng ChatItem để tạo danh sách ChatItemResponse
    //     var response = new List<ChatItemResponse>();
    //     foreach (var item in chatItems)
    //     {
    //         var chatItemResponse = new ChatItemResponse
    //         {
    //             Id = item.Id,
    //             NickName = await RoomChatHelper.GetNickNameAsync(getItemsHistoryRequest.ChatRoomId, item.UserName) ?? item.UserName,
    //             UserName = item.UserName,
    //             Timestamp = item.Timestamp,
    //             Type = item.Type,
    //             ReplyTo = item.ReplyTo,
    //             Unsend = item.Unsend,
    //             IsPin = item.IsPin,
    //             Data = item.ToResponseData()
    //         };
    //         response.Add(chatItemResponse);
    //     }

    //     return response;
    // }

    public async Task<List<MessageModel>> GetMessageHistoryAsync(GetHistoryRequest getItemsHistoryRequest)
    {
        // Tìm phòng chat dựa trên ID phòng
        var chatRoom = await _chatRooms
            .Find(room => room.Id == getItemsHistoryRequest.ChatRoomId)
            .FirstOrDefaultAsync();

        // Nếu không tìm thấy phòng chat, trả về danh sách rỗng
        if (chatRoom == null)
            return new List<MessageModel>();

        int limit = getItemsHistoryRequest.Limit;
        DateTime? timeFilter = getItemsHistoryRequest.Time;

        // Lọc các item theo thời gian và loại (nếu có)
        var filteredItems = chatRoom.Content
        .Where(item =>
            item.createdAt < timeFilter && // Lọc theo thời gian
            (getItemsHistoryRequest.Type == MessageType.All || (item.messageType & getItemsHistoryRequest.Type) != 0)) // Lọc theo loại nếu có
        .OrderByDescending(item => item.createdAt) // Sắp xếp theo thời gian giảm dần
        .Skip((getItemsHistoryRequest.Page - 1) * limit) // Bỏ qua số lượng bản ghi tương ứng với trang
        .Take(limit) // Lấy đúng số lượng tin nhắn cần thiết
        .ToList(); // Chuyển đổi thành danh sách

        return filteredItems;
    }

   
}