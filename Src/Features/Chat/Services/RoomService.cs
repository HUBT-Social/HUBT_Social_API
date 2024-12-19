

using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Driver;

namespace HUBT_Social_API.Features.Chat.Services;

public class RoomService : IRoomService
{

    private readonly IMongoCollection<ChatRoomModel> _chatRooms;

    public RoomService(IMongoCollection<ChatRoomModel> chatRooms)
    {
        _chatRooms = chatRooms;
    }
    // Update methods
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



    //Get methods
    public async Task<IEnumerable<ChatHistoryResponse>> GetChatHistoryAsync(GetItemsHistoryRequest getItemsHistoryRequest)
    {
        
        var chatItems = await GetItemsAsync(getItemsHistoryRequest);

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