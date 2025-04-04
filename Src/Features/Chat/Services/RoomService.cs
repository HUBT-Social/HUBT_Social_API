

using System.ComponentModel;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.Features.Chat.ChatHubs;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.Child;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.Collections;
using HUBTSOCIAL.Src.Features.Chat.Helpers;
using HUBTSOCIAL.Src.Features.Chat.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

namespace HUBT_Social_API.Features.Chat.Services;

public class RoomService : IRoomService
{
    private readonly IMongoCollection<ChatRoomModel> _chatRooms;
    public readonly IHubContext<ChatHub> _hubContext;
    private readonly IUserConnectionManager _userConnectionManager;
    private readonly UserManager<AUser> _userManager;

    public RoomService(
        IMongoCollection<ChatRoomModel> chatRooms,
        IHubContext<ChatHub> hubContext,
        IUserConnectionManager userConnectionManager,
        UserManager<AUser> userManager)
    {
        _chatRooms = chatRooms;
        _hubContext = hubContext;
        _userConnectionManager = userConnectionManager;
        _userManager = userManager;
    }

// Update methods
    /// <summary>
    /// Cập nhật tên nhóm cho phòng chat.
    /// </summary>
    /// <param name="id">ID của phòng chat cần cập nhật.</param>
    /// <param name="newName">Tên nhóm mới.</param>
    /// <returns>Trả về true nếu cập nhật thành công, ngược lại false.</returns>
    public async Task<bool> UpdateGroupNameAsync(string groupId, string userId, string newName)
    {
        try
        {
            // Kiểm tra xem phòng chat có tồn tại không
            var chatRoom = await _chatRooms.Find(c => c.Id == groupId).FirstOrDefaultAsync();
            if (chatRoom == null)
            {
                Console.WriteLine($"Chat room with ID {groupId} does not exist.");
                return false;
            }

            // Tạo định nghĩa cập nhật cho trường Name
            UpdateDefinition<ChatRoomModel> update = Builders<ChatRoomModel>.Update.Set(c => c.Name, newName);

            // Thực hiện cập nhật phòng chat dựa trên ID
            UpdateResult result = await _chatRooms.UpdateOneAsync(c => c.Id == groupId, update);

            if (result.ModifiedCount > 0)
            {
                // Gửi thông báo qua SignalR nếu cập nhật thành công
                await _hubContext.Clients.Group(groupId)
                .SendAsync("UpdateGroupName",  
                    new {
                        groupId = groupId, 
                        changerId = userId, 
                        newGroupName = newName
                        });
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
    ///     Cập nhật URL avatar cho phòng chat.
    /// </summary>
    /// <param name="id">ID của phòng chat cần cập nhật.</param>
    /// <param name="newUrl">URL mới cho avatar.</param>
    /// <returns>Trả về true nếu cập nhật thành công, ngược lại false.</returns>
    public async Task<bool> UpdateAvatarAsync(string groupId, string userId, string newUrl)
    {
        try
        {
            // Kiểm tra đầu vào
            if (string.IsNullOrEmpty(groupId) || string.IsNullOrEmpty(newUrl))
            {
                return false;
            }

            // Kiểm tra xem phòng chat có tồn tại không
            var chatRoom = await _chatRooms.Find(c => c.Id == groupId).FirstOrDefaultAsync();
            if (chatRoom == null)
            {
                return false;
            }

            // Tạo định nghĩa cập nhật cho trường AvatarUrl
            UpdateDefinition<ChatRoomModel> update = Builders<ChatRoomModel>.Update.Set(c => c.AvatarUrl, newUrl);

            // Thực hiện cập nhật phòng chat dựa trên ID
            UpdateResult result = await _chatRooms.UpdateOneAsync(c => c.Id == groupId, update);


            // Trả về true nếu số lượng bản ghi được cập nhật lớn hơn 0
            if (result.ModifiedCount > 0)
            {
                // Gửi thông báo qua SignalR nếu cập nhật thành công
                await _hubContext.Clients.Group(groupId)
                .SendAsync("UpdateGroupAvatar", 
                    new {
                        groupId = groupId, 
                        changerId = userId, 
                        newUrl = newUrl
                        });
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
    ///     Cập nhật biệt danh (nickname) của một thành viên trong phòng chat.
    /// </summary>
    /// <param name="roomId">ID của phòng chat.</param>
    /// <param name="userName">Tên người dùng cần cập nhật biệt danh.</param>
    /// <param name="newNickName">Biệt danh mới.</param>
    /// <returns>Trả về true nếu cập nhật thành công, ngược lại false.</returns>
    public async Task<bool> UpdateNickNameAsync(string groupId,string changerId ,string changedId, string newNickName)
    {
        try
        {
            // Tạo bộ lọc tìm phòng chat với `roomId` và `Participant` có `UserName` khớp
            var filter = Builders<ChatRoomModel>.Filter.And(
                Builders<ChatRoomModel>.Filter.Eq(r => r.Id, groupId),
                Builders<ChatRoomModel>.Filter.ElemMatch(r => r.Participant, p => p.UserId == changedId)
            );

            // Tạo định nghĩa cập nhật để thay đổi `NickName` của `Participant` khớp
            var update = Builders<ChatRoomModel>.Update.Set(
                "Participant.$.NickName", // `-1` đại diện cho phần tử được tìm qua `ElemMatch`
                newNickName
            );

            // Thực hiện cập nhật
            var result = await _chatRooms.UpdateOneAsync(filter, update);

            // Kiểm tra số lượng bản ghi bị thay đổi
            // Trả về true nếu số lượng bản ghi được cập nhật lớn hơn 0
            if (result.ModifiedCount > 0)
            {
                // Gửi thông báo qua SignalR nếu cập nhật thành công
                await _hubContext.Clients.Group(groupId)
                .SendAsync("UpdateNickName", 
                    new {
                        groupId = groupId,
                        changerId=changerId,
                        changedId = changedId, 
                        newNickName = newNickName});
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
    ///     Cập nhật vai trò của một thành viên trong phòng chat.
    /// </summary>
    /// <param name="roomId">ID của phòng chat.</param>
    /// <param name="userName">Tên người dùng cần cập nhật vai trò.</param>
    /// <param name="newParticipantRole">Vai trò mới cần gán.</param>
    /// <returns>Trả về true nếu cập nhật thành công, ngược lại false.</returns>
    public async Task<bool> UpdateParticipantRoleAsync(string groupId, string changerId ,string changedId, ParticipantRole newParticipantRole)
{
    try
    {
        // Tìm phòng chat chứa `roomId` và `Participant` có `UserName` khớp
        var filter = Builders<ChatRoomModel>.Filter.And(
            Builders<ChatRoomModel>.Filter.Eq(r => r.Id, groupId),
            Builders<ChatRoomModel>.Filter.ElemMatch(r => r.Participant, p => p.UserId == changedId)
        );

            // Kiểm tra sự tồn tại trước khi cập nhật
            var chatRoom = await _chatRooms.Find(filter).FirstOrDefaultAsync();
            if (chatRoom == null) return false;

            

            // Cập nhật trường `Role` của phần tử tìm được trong mảng `Participant`
        var update = Builders<ChatRoomModel>.Update.Set("Participant.$.Role", newParticipantRole);

        // Thực hiện cập nhật
        var result = await _chatRooms.UpdateOneAsync(filter, update);

        // Trả về true nếu số lượng bản ghi được cập nhật lớn hơn 0
            if(result.ModifiedCount > 0)
            {
                // Gửi thông báo qua SignalR nếu cập nhật thành công
                await _hubContext.Clients.Group(groupId)
                .SendAsync("UpdateParticipantRole", 
                    new {
                        groupId = groupId,
                        changerId = changerId,
                        changedId = changedId, 
                        newRole = newParticipantRole
                        });
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
    ///     Thêm một thành viên mới vào phòng chat.
    /// </summary>
    /// <param name="request">Yêu cầu thêm thành viên.</param>
    /// <returns>Trả về true nếu thêm thành công, ngược lại false.</returns>
    public async Task<bool> JoinRoomAsync(AddMemberRequest request,string AdderId)
    {
        try
        {
            var filter = Builders<ChatRoomModel>.Filter.Eq(r => r.Id, request.GroupId);
;
            var update = Builders<ChatRoomModel>.Update.AddToSet(r => r.Participant,request.Added);
            var result = await _chatRooms.UpdateOneAsync(filter, update);
            if (result.ModifiedCount > 0)
            {
                var connectionId = _userConnectionManager.GetConnectionId(request.Added.UserId);
                if (connectionId != null)
                {
                    await _hubContext.Groups.AddToGroupAsync(connectionId, request.GroupId);
                }
                // Gửi thông báo qua SignalR nếu cập nhật thành công
                await _hubContext.Clients.Group(request.GroupId)
                .SendAsync("UserJoinedGroup",
                    new {
                        groupId = request.GroupId,
                        AdderId = AdderId,
                        AddedId = request.Added.UserId
                        });
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
    ///     Xóa một thành viên khỏi phòng chat.
    /// </summary>
    /// <param name="request">Yêu cầu xóa thành viên.</param>
    /// <returns>Trả về true nếu xóa thành công, ngược lại false.</returns>
    public async Task<bool> KickMemberAsync(RemoveMemberRequest request,string KickerId)
    {
        try
        {
            var filter = Builders<ChatRoomModel>.Filter.Eq(r => r.Id, request.GroupId);

            var update = Builders<ChatRoomModel>.Update.PullFilter(r => r.Participant,
                p => p.UserId == request.KickedId);

            var result = await _chatRooms.UpdateOneAsync(filter, update);
            if (result.ModifiedCount > 0)
            {
                // Gửi thông báo qua SignalR nếu cập nhật thành công
                var connectionId = _userConnectionManager.GetConnectionId(request.KickedId);
                if (connectionId != null)
                {
                    await _hubContext.Groups.RemoveFromGroupAsync(connectionId, request.GroupId);
                }
                await _hubContext.Clients.Group(request.GroupId)
                .SendAsync("KickMember", 
                    new {
                        groupId = request.GroupId,
                        KickerId = KickerId,
                        KickedId = request.KickedId
                        });
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
    public async Task<bool> LeaveRoomAsync(string groupId,string userId)
    {
        try
        {
            var filter = Builders<ChatRoomModel>.Filter.Eq(r => r.Id, groupId);


            var update = Builders<ChatRoomModel>.Update.PullFilter(r => r.Participant,
                p => p.UserId == userId);

            var result = await _chatRooms.UpdateOneAsync(filter, update);
            if (result.ModifiedCount > 0)
            {
                // Gửi thông báo qua SignalR nếu cập nhật thành công
                var connectionId = _userConnectionManager.GetConnectionId(userId);
                if (connectionId != null)
                {
                    await _hubContext.Groups.RemoveFromGroupAsync(connectionId, groupId);
                }
                await _hubContext.Clients.Group(groupId)
                    .SendAsync("LeaveRoom",
                        new { 
                            groupId = groupId,
                            userId = userId
                        }
                    );
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
    
  

    public async Task<List<MessageModel>> GetMessageHistoryAsync(GetHistoryRequest request)
    {
        if (request.Limit <= 0) return new List<MessageModel>();

        try
        {
            // Tính toán vị trí bắt đầu và số lượng phần tử cần lấy
            int totalMessages = await _chatRooms
                .Find(cr => cr.Id == request.ChatRoomId)
                .Project(cr => cr.Content.Count)
                .FirstOrDefaultAsync();
            
            if(totalMessages == request.CurrentQuantity)
            {
                return new List<MessageModel>();
            }

            int startIndex = totalMessages - request.CurrentQuantity - request.Limit;
            int count = request.Limit;

            if(startIndex < 0)
            {
                startIndex = 0;
                count = totalMessages-request.CurrentQuantity;
            }

            Console.WriteLine("Vị trí lấy: ",startIndex.ToString());
            
            // Truy vấn tin nhắn với Slice
            var projection = Builders<ChatRoomModel>.Projection
                .Include(cr => cr.Content)
                .Slice(cr => cr.Content, startIndex, count);

            var result = await _chatRooms
                .Find(cr => cr.Id == request.ChatRoomId)
                .Project<ChatRoomModel>(projection)
                .FirstOrDefaultAsync();

            return result?.Content ?? new List<MessageModel>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi lấy tin nhắn: {ex.Message}");
            return new List<MessageModel>();
        }
    }


    public async Task<List<ChatUserResponse>> GetRoomUserAsync(string groupId)
    {
        var chatRoom = await _chatRooms
            .Find(room => room.Id == groupId)
            .FirstOrDefaultAsync();


        if (chatRoom == null || chatRoom.Participant == null)
        {
            return new List<ChatUserResponse>();
        }

        // Lấy thêm thông tin user nếu cần
        var participants = chatRoom.Participant;
        List<ChatUserResponse> res = new List<ChatUserResponse>();
        foreach (var participant in participants)
        {

            ChatUserResponse chatUserResponse = new ChatUserResponse();
            string? userAvatar = await UserHelper.GetAvatarUserById(_userManager,participant.UserId);
            chatUserResponse.id = participant.UserId;
            chatUserResponse.name = participant.NickName;
            chatUserResponse.profilePhoto = userAvatar??participant.DefaultAvatarImage;
            res.Add(chatUserResponse);
        }

        return res;
    }
}