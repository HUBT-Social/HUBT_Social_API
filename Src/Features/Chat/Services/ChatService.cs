using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.DTOs;
using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Driver;

namespace HUBT_Social_API.Features.Chat.Services;

public class ChatService : IChatService
{
    private readonly IMongoCollection<ChatRoomModel> _chatRooms;

    public ChatService(IMongoCollection<ChatRoomModel> chatRooms)
    {
        _chatRooms = chatRooms;
    }

    /// <summary>
    /// Thêm một nhóm chat mới vào cơ sở dữ liệu.
    /// </summary>
    /// <param name="newRoomModel">
    /// Đối tượng chứa thông tin của nhóm chat mới, bao gồm tên nhóm, danh sách thành viên, avatar, v.v.
    /// </param>
    /// <returns>
    /// ID của nhóm chat vừa được thêm nếu thành công. 
    /// Nếu không thành công, trả về null.
    /// </returns>
    /// <example>
    /// Ví dụ:
    /// var groupId = await CreateGroupAsync(new ChatRoomModel { Name = "Group A" });
    /// </example>
    public async Task<string?> CreateGroupAsync(ChatRoomModel newRoomModel)
    {
        try
        {
            await _chatRooms.InsertOneAsync(newRoomModel); 
            return newRoomModel.Id;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Xóa một nhóm chat khỏi cơ sở dữ liệu dựa trên ID.
    /// </summary>
    /// <param name="id">
    /// ID của nhóm chat cần xóa.
    /// </param>
    /// <returns>
    /// Trả về true nếu xóa thành công, ngược lại false.
    /// </returns>
    /// <example>
    /// Ví dụ:
    /// var isDeleted = await DeleteGroupAsync("group123");
    /// </example>
    public async Task<bool> DeleteGroupAsync(string id)
    {
        try
        {
            DeleteResult result = await _chatRooms.DeleteOneAsync(c => c.Id == id); 
            return result.DeletedCount > 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Lấy thông tin chi tiết của một nhóm chat theo ID.
    /// </summary>
    /// <param name="id">
    /// ID của nhóm chat cần lấy thông tin.
    /// </param>
    /// <returns>
    /// Đối tượng `ChatRoomModel` chứa thông tin chi tiết của nhóm chat. 
    /// Nếu không tìm thấy, trả về null.
    /// </returns>
    /// <example>
    /// Ví dụ:
    /// var group = await GetGroupByIdAsync("group123");
    /// </example>
    public async Task<ChatRoomModel> GetGroupByIdAsync(string id)
    {
        return await _chatRooms.Find(c => c.Id == id).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Tìm kiếm các nhóm chat theo từ khóa.
    /// </summary>
    /// <param name="keyword">
    /// Từ khóa cần tìm kiếm, áp dụng cho tên nhóm chat.
    /// </param>
    /// <returns>
    /// Danh sách các nhóm chat khớp với từ khóa tìm kiếm, chứa các trường cần thiết.
    /// Nếu từ khóa trống, trả về danh sách rỗng.
    /// </returns>
    /// <example>
    /// Ví dụ:
    /// var groups = await SearchGroupsAsync("team");
    /// </example>
    public async Task<List<SearchChatRoomReponse>> SearchGroupsAsync(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return new List<SearchChatRoomReponse>();

        var filter = Builders<ChatRoomModel>.Filter.Regex(
            cr => cr.Name, 
            new MongoDB.Bson.BsonRegularExpression(keyword, "i")
        );

        var projection = Builders<ChatRoomModel>.Projection.Expression(cr => new SearchChatRoomReponse
        {
            Id = cr.Id,
            GroupName = cr.Name,
            AvatarUrl = cr.AvatarUrl,
            TotalNumber = cr.Participant.Count,
        });

        return await _chatRooms
            .Find(filter)
            .Project(projection)
            .Limit(10)
            .ToListAsync();
    }

    /// <summary>
    /// Lấy danh sách tất cả các nhóm chat từ cơ sở dữ liệu.
    /// </summary>
    /// <returns>
    /// Danh sách các đối tượng `ChatRoomModel` chứa thông tin của tất cả nhóm chat.
    /// </returns>
    /// <example>
    /// Ví dụ:
    /// var allRooms = await GetAllRoomsAsync();
    /// </example>
    public async Task<List<ChatRoomModel>> GetAllRoomsAsync()
    {
        return await _chatRooms.Find(_ => true).ToListAsync();
    }
    /// <summary>
    /// Lấy tất cả phòng chat có chứa ID người dùng.
    /// </summary>
    /// <param name="userId">
    /// ID của người dùng, được truyền vào sau khi thực hiện decode từ Access Token.
    /// </param>
    /// <returns>
    /// Danh sách các phòng chat mà người dùng tham gia. 
    /// Nếu không có phòng nào, trả về danh sách rỗng.
    /// </returns>
    /// 
    /// <example>
    /// Ví dụ: 
    /// var rooms = await GetRoomsByIdUserAsync("user123");
    /// </example>
   public async Task<List<ChatRoomModel>> GetRoomsByUserNameAsync(string userName)
    {
        // Tìm các room chứa userName trong danh sách Participant
        var chatRooms = await _chatRooms
            .Find(room => room.Participant.Any(p => p.UserName == userName))
            .ToListAsync();

        // Trả về danh sách rỗng nếu không tìm thấy
        return chatRooms ?? new List<ChatRoomModel>();
    }


}