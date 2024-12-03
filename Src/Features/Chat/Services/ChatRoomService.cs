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
                Id = Guid.NewGuid().ToString(),
                Name = createGroupRequest.GroupName,
                AvatarUrl = LocalValue.Get(KeyStore.DefaultUserImage),
                UserIds = createGroupRequest.UserIds,
                CreatedAt = DateTime.Now
            };

            await _chatRooms.InsertOneAsync(newChatRoom); 
            return newChatRoom.Id;
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
            UpdateResult result = await _chatRooms.UpdateOneAsync(c => c.Id == id, update);

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
            DeleteResult result = await _chatRooms.DeleteOneAsync(c => c.Id == id); // Xóa nhóm theo ID
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
        return await _chatRooms.Find(c => c.Id == id).FirstOrDefaultAsync(); // Tìm nhóm theo ID
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
            Id = cr.Id,
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
}