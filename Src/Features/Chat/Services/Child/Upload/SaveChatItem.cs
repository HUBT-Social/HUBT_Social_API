using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Driver;

namespace HUBT_Social_API.Features.Chat.Services.Child;

public static class SaveChatItem
{
    public static async Task<UpdateResult> Save(IMongoCollection<ChatRoomModel> chatRooms, ChatRoomModel Room,
        MessageModel Message)
    {
        // Tạo filter cho GroupId và UserName
        FilterDefinition<ChatRoomModel> filter = Builders<ChatRoomModel>.Filter.And(
            Builders<ChatRoomModel>.Filter.Eq(cr => cr.Id, Room.Id),
            Builders<ChatRoomModel>.Filter.ElemMatch(cr => cr.Participant, p => p.UserName == Message.SentBy)
        );

        // Tạo update để cập nhật LastInteractionTime
        UpdateDefinition<ChatRoomModel> updateLastInteractionTime = Builders<ChatRoomModel>.Update
            .Set(cr => cr.LastInteractionTime, DateTime.Now); // Cập nhật thời gian tương tác gần nhất

        // Tạo update để thêm tin nhắn mới vào ChatItems
        UpdateDefinition<ChatRoomModel> updateChatItems = Builders<ChatRoomModel>.Update
            .Push(cr => cr.Content, Message); // Thêm tin nhắn mới vào danh sách ChatItems


        var updateResult = await chatRooms.UpdateOneAsync(filter,
            Builders<ChatRoomModel>.Update.Combine(updateLastInteractionTime, updateChatItems));


        return updateResult; // Trả về kết quả cập nhật
    }
}