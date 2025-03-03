using HUBT_Social_API.Features.Chat.Services.Child;
using HUBTSOCIAL.Src.Features.Chat.Collections;
using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Driver;

namespace HUBTSOCIAL.Src.Features.Chat.Helpers
{
    public static class RoomChatHelper
    {
        private static IMongoCollection<ChatRoomModel> _chatRooms;
        private static IMongoCollection<ChatHistory> _chatHistory;

        // Constructor để gán collection (_chatRooms) khi khởi tạo
        public static void Initialize(IMongoCollection<ChatRoomModel> chatRooms,IMongoCollection<ChatHistory> chatHistory)
        {
            _chatRooms = chatRooms;
            _chatHistory = chatHistory;
        }

        // Phương thức lấy role của participant
        public static async Task<ParticipantRole?> GetRoleAsync(string roomId, string userId)
        {
            var filter = Builders<ChatRoomModel>.Filter.And(
                Builders<ChatRoomModel>.Filter.Eq(r => r.Id, roomId),
                Builders<ChatRoomModel>.Filter.ElemMatch(r => r.Participant, p => p.UserId == userId)
            );

            var projection = Builders<ChatRoomModel>.Projection.Expression(r =>
                r.Participant.FirstOrDefault(p => p.UserId == userId));

            var participant = await _chatRooms
                .Find(filter)
                .Project(projection)
                .FirstOrDefaultAsync();

            return participant?.Role;
        }

        // Phương thức lấy nickname của participant
        public static async Task<string?> GetNickNameAsync(string roomId, string UserId)
        {
            var filter = Builders<ChatRoomModel>.Filter.And(
                Builders<ChatRoomModel>.Filter.Eq(r => r.Id, roomId),
                Builders<ChatRoomModel>.Filter.ElemMatch(r => r.Participant, p => p.UserId == UserId)
            );

            var projection = Builders<ChatRoomModel>.Projection.Expression(r =>
                r.Participant.FirstOrDefault(p => p.UserId == UserId));

            var participant = await _chatRooms
                .Find(filter)
                .Project(projection)
                .FirstOrDefaultAsync();

            return participant?.NickName;
        }

        // Phương thức lấy thông tin của một message
        public static async Task<MessageModel?> GetInfoMessageAsync(string roomId, string messageId)
        {
            var filter0 = Builders<ChatRoomModel>.Filter.Eq(cr => cr.Id, roomId);
            var chatRoom = await _chatRooms.Find(filter0).FirstOrDefaultAsync();

            if (chatRoom == null)
                return null;
            var parts = messageId.Split('_');
            string blockId =  parts.Length == 2 ? parts[1] : null;

            var hotMessage = chatRoom.HotContent
                .FirstOrDefault(m => m.id == messageId);

            if (hotMessage != null) return hotMessage;

            // 2. Tìm trong ChatHistory dựa trên BlockId
            var filter = Builders<ChatHistory>.Filter.And(
                Builders<ChatHistory>.Filter.Eq(ch => ch.RoomId, roomId),
                Builders<ChatHistory>.Filter.ElemMatch(ch => ch.HistoryChat, b => b.BlockId == blockId)
            );

            var projection = Builders<ChatHistory>.Projection
                .Include(ch => ch.HistoryChat[-1].Data); // Chỉ lấy block cần thiết

            var chatHistory = await _chatHistory
                .Find(filter)
                .Project<ChatHistory>(projection)
                .FirstOrDefaultAsync();

            if (chatHistory == null || !chatHistory.HistoryChat.Any()) return null;

            var block = chatHistory.HistoryChat.FirstOrDefault(b => b.BlockId == blockId);
            return block?.Data.FirstOrDefault(m => m.id == messageId);
        }
        public static async Task<List<string>> GetUserGroupConnected(string UserId)
        {
            // Tạo bộ lọc để tìm các phòng chat có chứa userName trong danh sách Participant
            var filter = Builders<ChatRoomModel>.Filter.ElemMatch(
                cr => cr.Participant,
                p => p.UserId == UserId
            );

            // Lấy tất cả các phòng chat phù hợp với bộ lọc, sắp xếp theo LastInteractionTime
            var chatRooms = await _chatRooms
                .Find(filter)
                //.SortByDescending(cr => cr.LastInteractionTime)
                .ToListAsync();

            // Lấy danh sách các Id của phòng chat
            var response = chatRooms.Select(cr => cr.Id).ToList();

            return response;
        }
        
    }
}
