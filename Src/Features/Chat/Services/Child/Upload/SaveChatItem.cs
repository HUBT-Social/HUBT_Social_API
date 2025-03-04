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
        public static async Task<UpdateResult> Save(
            IMongoCollection<ChatRoomModel> chatRooms,
            IMongoCollection<ChatHistory> chatHistory,
            string RoomId,
            MessageModel message)
        {
            try
            {
                // Kiểm tra tham số đầu vào
                ArgumentNullException.ThrowIfNullOrEmpty(RoomId, nameof(RoomId));
                ArgumentNullException.ThrowIfNull(message, nameof(message));
                ArgumentNullException.ThrowIfNull(chatRooms, nameof(chatRooms));
                ArgumentNullException.ThrowIfNull(chatHistory, nameof(chatHistory));

                // Đảm bảo message.createdAt có giá trị
                if (message.createdAt == default)
                {
                    Console.WriteLine("message.createdAt is not set, assigning current UTC time");
                    message.createdAt = DateTime.UtcNow;
                }

                // Tạo bộ lọc tìm phòng chat
                var filter = Builders<ChatRoomModel>.Filter.Eq(cr => cr.Id, RoomId);
                var room = await chatRooms.Find(filter).FirstOrDefaultAsync();

                if (room == null)
                {
                    Console.WriteLine($"Room with Id {RoomId} not found");
                    throw new InvalidOperationException($"Chat room with Id {RoomId} does not exist");
                }

                // Đảm bảo các danh sách không null
                room.HotContent ??= new List<MessageModel>();
                room.PageReference ??= new List<PageReference>();
                room.CachePageReference ??= new PageReference { BlockId = string.Empty };

                Console.WriteLine($"HotContent count before update: {room.HotContent.Count}");

                // Cập nhật LastInteractionTime
                var updateLastInteractionTime = Builders<ChatRoomModel>.Update
                    .Set(cr => cr.LastInteractionTime, DateTime.UtcNow);

                UpdateResult updateResult;

                if (room.HotContent.Count + 1 >= 21)
                {
                    var newBlockId = Guid.NewGuid().ToString();
                    var newPageRef = new PageReference
                    {
                        BlockId = newBlockId,
                        PreBlockId = room.CachePageReference.BlockId
                    };

                    // Tạo block mới từ HotContent hiện tại + tin nhắn mới
                    var blockMessages = new List<MessageModel>(room.HotContent) { message };
                    var block = new ChatPage
                    {
                        BlockId = newBlockId,
                        Data = blockMessages,
                        StartTime = blockMessages.Min(m => m.createdAt),
                        EndTime = blockMessages.Max(m => m.createdAt)
                    };

                    // Cập nhật ChatHistory
                    var historyFilter = Builders<ChatHistory>.Filter.Eq(ch => ch.RoomId, RoomId);
                    var existingHistory = await chatHistory.Find(historyFilter).FirstOrDefaultAsync();

                    if (existingHistory == null)
                    {
                        var newHistory = new ChatHistory
                        {
                            RoomId = RoomId,
                            HistoryChat = new List<ChatPage> { block }
                        };
                        await chatHistory.InsertOneAsync(newHistory);
                        Console.WriteLine($"Created new ChatHistory for RoomId {RoomId}");
                    }
                    else
                    {
                        existingHistory.HistoryChat ??= new List<ChatPage>();
                        var historyUpdate = Builders<ChatHistory>.Update.Push(ch => ch.HistoryChat, block);
                        await chatHistory.UpdateOneAsync(historyFilter, historyUpdate);
                        Console.WriteLine($"Updated ChatHistory for RoomId {RoomId}");
                    }

                    // Cập nhật ChatRoom
                    var update = Builders<ChatRoomModel>.Update.Combine(
                        updateLastInteractionTime,
                        Builders<ChatRoomModel>.Update.Push(cr => cr.PageReference, room.CachePageReference),
                        Builders<ChatRoomModel>.Update.Set(cr => cr.CachePageReference, newPageRef),
                        Builders<ChatRoomModel>.Update.Set(cr => cr.HotContent, new List<MessageModel> { message })
                    );

                    updateResult = await chatRooms.UpdateOneAsync(filter, update);
                }
                else
                {
                    var updateChatItems = Builders<ChatRoomModel>.Update.Push(cr => cr.HotContent, message);
                    var update = Builders<ChatRoomModel>.Update.Combine(updateLastInteractionTime, updateChatItems);
                    updateResult = await chatRooms.UpdateOneAsync(filter, update);
                }

                // Log kết quả
                Console.WriteLine($"MatchedCount: {updateResult.MatchedCount}, ModifiedCount: {updateResult.ModifiedCount}");
                if (updateResult.MatchedCount == 0)
                {
                    Console.WriteLine($"No room matched for RoomId {RoomId}. Check filter or database state.");
                }
                else if (updateResult.ModifiedCount > 0)
                {
                    Console.WriteLine($"Successfully updated room {RoomId}");
                }

                return updateResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi lưu tin nhắn: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }
    }
}

public class ChatHistory
{
    [BsonId]
    public string RoomId { get; set; }
    public List<ChatPage> HistoryChat { get; set; } = new();
}
