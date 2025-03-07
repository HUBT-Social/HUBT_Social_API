using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace HUBT_Social_API.Features.Chat.Services.Child
{
    public static class SaveChatItem
    {
        public static async Task<UpdateResult> Save(IMongoCollection<ChatRoomModel> chatRooms, string roomId, MessageModel message)
        {
            

            try
            {
                if (chatRooms == null)
                    throw new ArgumentNullException(nameof(chatRooms), "Chat rooms collection cannot be null.");
                if (string.IsNullOrEmpty(roomId))
                    throw new ArgumentNullException(nameof(roomId), "Room ID cannot be null or empty.");
                if (message == null)
                    throw new ArgumentNullException(nameof(message), "Message cannot be null.");


                // 📌 Tạo bộ lọc tìm phòng chat theo `Room.Id`
                var filter = Builders<ChatRoomModel>.Filter.Eq(cr => cr.Id, roomId);

                var updateLastInteractionTime = Builders<ChatRoomModel>.Update
                    .Set(cr => cr.LastInteractionTime, DateTime.UtcNow);

                // ✅ Thêm tin nhắn mới vào `Content`
                var updateChatItems = Builders<ChatRoomModel>.Update
                    .Push(cr => cr.Content, message);

                // 🛠 Kết hợp các cập nhật và thực hiện lệnh UpdateOne
                var updateResult = await chatRooms.UpdateOneAsync(
                    filter,
                    Builders<ChatRoomModel>.Update.Combine(updateLastInteractionTime, updateChatItems)
                );

                // ✅ In kết quả để kiểm tra
                Console.WriteLine($"🔹 MatchedCount: {updateResult.MatchedCount}, ModifiedCount: {updateResult.ModifiedCount}");

                if (updateResult.ModifiedCount > 0)
                {
                    Console.WriteLine("✅");
                }
                else
                {
                    Console.WriteLine("⚠️");
                }

                return updateResult;
            }
            catch
            {
                Console.WriteLine($"❌");
                throw;
            }
        }
    }
}
