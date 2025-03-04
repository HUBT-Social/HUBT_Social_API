
using System.Text.Json;
using FireSharp.Extensions;
using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace HUBT_Social_API.Features.Chat.Services.Child
{
    public static class SaveChatItem
{
    public static async Task<UpdateResult> Save(IMongoCollection<ChatRoomModel> chatRooms,IMongoCollection<ChatHistory> chatHistory, string RoomId, MessageModel message)
    {
        try
        {
            // 📌 Tạo bộ lọc tìm phòng chat theo `Room.Id` và đảm bảo `UserName` có trong `Participant`
            var filter = Builders<ChatRoomModel>.Filter.And(
                Builders<ChatRoomModel>.Filter.Eq(cr => cr.Id, RoomId)
            );
            var room = await chatRooms.Find(filter).FirstOrDefaultAsync();
            if (room == null);

   
            // ✅ Cập nhật `LastInteractionTime`
            var updateLastInteractionTime = Builders<ChatRoomModel>.Update
                .Set(cr => cr.LastInteractionTime, DateTime.UtcNow);

            UpdateResult updateResult;

            // Kiểm tra và xử lý HotContent
            if (room.HotContent.Count + 1 == 21)
            {
                var newBlockId = Guid.NewGuid().ToString();
                var newPageRef = new PageReference
                {
                    BlockId = newBlockId,
                    PreBlockId = room.PreBlockId
                };

                // Tạo block mới từ HotContent hiện tại + tin nhắn mới
                var blockMessages = new List<MessageModel>(room.HotContent);
                var block = new ChatPage
                {
                    BlockId = newBlockId,
                    Data = blockMessages,
                    StartTime = blockMessages.Min(m => m.createdAt),
                    EndTime = blockMessages.Max(m => m.createdAt)
                };

                // Kiểm tra xem ChatHistory đã tồn tại chưa
                var historyFilter = Builders<ChatHistory>.Filter.Eq(ch => ch.RoomId, RoomId);
                var existingHistory = await chatHistory.Find(historyFilter).FirstOrDefaultAsync();

                if (existingHistory == null)
                {
                    // Tạo ChatHistory mới nếu chưa tồn tại
                    var newHistory = new ChatHistory
                    {
                        RoomId = RoomId,
                        HistoryChat = new List<ChatPage> { block }
                    };
                    await chatHistory.InsertOneAsync(newHistory);
                }
                else
                {
                    // Đẩy block vào ChatHistory nếu đã tồn tại
                    var historyUpdate = Builders<ChatHistory>.Update
                        .Push(ch => ch.HistoryChat, block);
                    await chatHistory.UpdateOneAsync(historyFilter, historyUpdate);
                }

                // Cập nhật ChatRoom: thêm PageReference, reset HotContent
                var update = Builders<ChatRoomModel>.Update.Combine(
                    updateLastInteractionTime,
                    Builders<ChatRoomModel>.Update.Push(cr => cr.PageReference, newPageRef),
                    Builders<ChatRoomModel>.Update.Set(cr => cr.PreBlockId, newBlockId),
                    Builders<ChatRoomModel>.Update.Set(cr => cr.HotContent, new List<MessageModel>(){message})
                );

                updateResult = await chatRooms.UpdateOneAsync(filter, update);
            }
            else
            {
                // ✅ Thêm tin nhắn mới vào `HotContent` nếu chưa đầy
                var updateChatItems = Builders<ChatRoomModel>.Update
                    .Push(cr => cr.HotContent, message);

                // 🛠 Kết hợp các cập nhật
                var update = Builders<ChatRoomModel>.Update.Combine(
                    updateLastInteractionTime,
                    updateChatItems
                );

                updateResult = await chatRooms.UpdateOneAsync(filter, update);
            }

            // ✅ In kết quả để kiểm tra
            Console.WriteLine($"🔹 MatchedCount: {updateResult.MatchedCount}, ModifiedCount: {updateResult.ModifiedCount}");

            if (updateResult.ModifiedCount > 0)
            {
                Console.WriteLine("✅ Dữ liệu đã đượcđược cập nhật: ");
            }
            else
            {
                Console.WriteLine("⚠️ Không có dữ liệu nào được cập nhật. Kiểm tra lại `filter` hoặc `update`.");
            }

            return updateResult;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Lỗi khi lưu tin nhắn: {ex.Message}");
            throw;
        }
    }
}
public class ChatHistory
{
    [BsonId]
    public string RoomId { get; set; }
    public List<ChatPage> HistoryChat { get; set; } = new();
}

}