using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.DTOs;
using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Driver;

namespace HUBT_Social_API.Features.Chat.Services;

public class ChatRoomService : IChatRoomService
{
    private readonly IMongoCollection<ChatRoomModel> _chatRooms;

    public ChatRoomService(IMongoCollection<ChatRoomModel> chatRooms)
    {
        _chatRooms = chatRooms;
    }

    // Thêm nhóm chat mới
    public async Task<string?> CreateGroupAsync(CreateGroupRequest createGroupRequest)
    {
        try
        {
            ChatRoomModel newChatRoom = new()
            {
                ChatRoomId = Guid.NewGuid().ToString(),
                Name = createGroupRequest.GroupName,
                AvatarUrl = LocalValue.Get(KeyStore.DefaultUserImage),
                UserIds = createGroupRequest.UserIds,
                CreatedAt = DateTime.Now
            };

            await _chatRooms.InsertOneAsync(newChatRoom); 
            return newChatRoom.ChatRoomId;
        }
        catch
        {
            return null;
        }
        
    }

    // Cập nhật tên nhóm
    public async Task<bool> UpdateGroupNameAsync(string id, string newName)
    {
        try
        {
            UpdateDefinition<ChatRoomModel> update = Builders<ChatRoomModel>.Update.Set(c => c.Name, newName); 
            UpdateResult result = await _chatRooms.UpdateOneAsync(c => c.ChatRoomId == id, update);

            return result.ModifiedCount > 0; 
        }
        catch
        {
            return false;
        }
        
    }

    // Xóa nhóm chat
    public async Task<bool> DeleteGroupAsync(string id)
    {
        try
        {
            DeleteResult result = await _chatRooms.DeleteOneAsync(c => c.ChatRoomId == id); // Xóa nhóm theo ID
            return result.DeletedCount > 0; // Kiểm tra xem có nhóm nào bị xóa không
        }
        catch
        {
            return false;
        }
        
    }

    // Lấy thông tin nhóm theo ID
    public async Task<ChatRoomModel> GetGroupByIdAsync(string id)
    {
        return await _chatRooms.Find(c => c.ChatRoomId == id).FirstOrDefaultAsync(); // Tìm nhóm theo ID
    }
    // Tim kiem group
    public async Task<List<SearchChatRoomReponse>> SearchGroupsAsync(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return new List<SearchChatRoomReponse>(); // Trả về danh sách rỗng nếu từ khóa trống

        // Sử dụng regex để tìm kiếm theo keyword
        var filter = Builders<ChatRoomModel>.Filter.Regex(
            cr => cr.Name, 
            new MongoDB.Bson.BsonRegularExpression(keyword, "i")
        );

        // Chỉ lấy các trường cần thiết và giới hạn số lượng kết quả
        var projection = Builders<ChatRoomModel>.Projection.Expression(cr => new SearchChatRoomReponse
        {
            ChatRoomId = cr.ChatRoomId,
            GroupName = cr.Name,
            AvatarUrl = cr.AvatarUrl,
            TotalNumber = cr.UserIds.Count,
        });

        return await _chatRooms
            .Find(filter)
            .Project(projection)
            .Limit(10) // Giới hạn kết quả trả về là 10 nếu muốn nhiều hơn thì có thể truyền tham số đầu vào
            .ToListAsync();
    }


    // Lấy tất cả nhóm chat
    public async Task<List<ChatRoomModel>> GetAllGroupsAsync()
    {
        return await _chatRooms.Find(_ => true).ToListAsync(); // Lấy tất cả nhóm từ MongoDB
    }












    public async Task<IEnumerable<ChatHistoryResponse>> GetChatHistoryAsync(GetChatHistoryRequest getChatHistoryRequest)
    {
        var chatItems = await GetItemsAsync(getChatHistoryRequest);

        var response = chatItems.Select(item => new ChatHistoryResponse
        {
            Id = item.Id,
            SenderId = item.SenderId,
            Timestamp = item.Timestamp,
            Type = item.Type,
            Data = item.ToResponseData()
        }).ToList();

        return response;
    }

        public async Task<List<ChatItem>> GetItemsAsync(GetChatHistoryRequest getChatHistoryRequest)
        {
            // Giả lập data từ `ChatRoomModel` (thay thế bằng kết nối DB thực tế)
            var chatRoom = _chatRooms.Find(room => room.ChatRoomId == getChatHistoryRequest.ChatRoomId).FirstOrDefault();

            if (chatRoom == null)
                return new List<ChatItem>();

            // Lọc các item cũ hơn `beforeTimestamp`
            var filteredItems = chatRoom.ChatItems
                .Where(item => getChatHistoryRequest.Time == null || item.Timestamp < getChatHistoryRequest.Time)
                .OrderByDescending(item => item.Timestamp)
                .Take(getChatHistoryRequest.Limit)
                .ToList();

            return filteredItems;
        }




public class ChatHistoryResponse
{
    public string Id { get; set; }
    public string SenderId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Type { get; set; }
    public object? Data { get; set; }
}



}