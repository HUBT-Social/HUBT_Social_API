
using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Driver;

namespace HUBT_Social_API.Features.Chat.Services.Child
{
    public static class SaveChatItem
    {
        public static async Task<UpdateResult> Save(IMongoCollection<ChatRoomModel> chatRooms, ChatRoomModel Room, ChatItem chatItem)
        {
            // Tạo filter cho GroupId và UserName
            FilterDefinition<ChatRoomModel> filter = Builders<ChatRoomModel>.Filter.And(
                Builders<ChatRoomModel>.Filter.Eq(cr => cr.Id, Room.Id),
                Builders<ChatRoomModel>.Filter.ElemMatch(cr => cr.Participant, p => p.UserName == chatItem.UserName)
            );

            // Tạo update để cập nhật LastInteractionTime
            UpdateDefinition<ChatRoomModel> updateLastInteractionTime = Builders<ChatRoomModel>.Update
                .Set(cr => cr.LastInteractionTime, DateTime.Now); // Cập nhật thời gian tương tác gần nhất

            // Tạo update để thêm tin nhắn mới vào ChatItems
            UpdateDefinition<ChatRoomModel> updateChatItems = Builders<ChatRoomModel>.Update
                .Push(cr => cr.ChatItems, chatItem); // Thêm tin nhắn mới vào danh sách ChatItems

            UpdateResult updateResult;

            // Nếu ChatItems có nhiều hơn 1000 tin nhắn, xóa bớt các tin nhắn cũ nhất
            if (Room.ChatItems.Count >= 1000)
            {
                // Sắp xếp và lấy 1000 tin nhắn mới nhất
                List<ChatItem> updatedChatItems = Room.ChatItems.OrderByDescending(c => c.Timestamp).Take(1000).ToList();

                UpdateDefinition<ChatRoomModel> updateLimitItems = Builders<ChatRoomModel>.Update
                    .Set(cr => cr.ChatItems, updatedChatItems); // Cập nhật lại ChatItems với 1000 tin nhắn mới nhất

                // Cập nhật dữ liệu phòng chat với cả thời gian và ChatItems
                updateResult = await chatRooms.UpdateOneAsync(filter, Builders<ChatRoomModel>.Update.Combine(updateLastInteractionTime, updateChatItems, updateLimitItems));
            }
            else
            {
                // Cập nhật chỉ có LastInteractionTime và thêm tin nhắn mới vào ChatItems nếu số lượng chưa vượt quá 1000
                updateResult = await chatRooms.UpdateOneAsync(filter, Builders<ChatRoomModel>.Update.Combine(updateLastInteractionTime, updateChatItems));
            }

            return updateResult; // Trả về kết quả cập nhật
        }
    }
}