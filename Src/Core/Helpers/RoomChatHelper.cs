using HUBTSOCIAL.Src.Features.Chat.Collections;
using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Driver;

namespace HUBTSOCIAL.Src.Features.Chat.Helpers
{
    public static class RoomChatHelper
    {
        private static IMongoCollection<ChatRoomModel> _chatRooms;

        // Constructor để gán collection (_chatRooms) khi khởi tạo
        public static void Initialize(IMongoCollection<ChatRoomModel> chatRooms)
        {
            _chatRooms = chatRooms;
        }

        // Phương thức lấy role của participant
        public static async Task<ParticipantRole?> GetRoleAsync(string roomId, string userName)
        {
            var filter = Builders<ChatRoomModel>.Filter.And(
                Builders<ChatRoomModel>.Filter.Eq(r => r.Id, roomId),
                Builders<ChatRoomModel>.Filter.ElemMatch(r => r.Participant, p => p.UserName == userName)
            );

            var projection = Builders<ChatRoomModel>.Projection.Expression(r =>
                r.Participant.FirstOrDefault(p => p.UserName == userName));

            var participant = await _chatRooms
                .Find(filter)
                .Project(projection)
                .FirstOrDefaultAsync();

            return participant?.Role;
        }

        // Phương thức lấy nickname của participant
        public static async Task<string?> GetNickNameAsync(string roomId, string userName)
        {
            var filter = Builders<ChatRoomModel>.Filter.And(
                Builders<ChatRoomModel>.Filter.Eq(r => r.Id, roomId),
                Builders<ChatRoomModel>.Filter.ElemMatch(r => r.Participant, p => p.UserName == userName)
            );

            var projection = Builders<ChatRoomModel>.Projection.Expression(r =>
                r.Participant.FirstOrDefault(p => p.UserName == userName));

            var participant = await _chatRooms
                .Find(filter)
                .Project(projection)
                .FirstOrDefaultAsync();

            return participant?.NickName;
        }

        // Phương thức lấy thông tin của một message
        public static async Task<MessageModel?> GetInfoMessageAsync(string roomId, string messageId)
        {
            var filter = Builders<ChatRoomModel>.Filter.Eq(cr => cr.Id, roomId);
            var chatRoom = await _chatRooms.Find(filter).FirstOrDefaultAsync();

            if (chatRoom == null)
                return null;

            var message = chatRoom.Content.FirstOrDefault(ci => ci.Id == messageId);

            return message;
        }
        public static async Task<List<string>> GetUserGroupConnected(string userName)
        {
            // Tạo bộ lọc để tìm các phòng chat có chứa userName trong danh sách Participant
            var filter = Builders<ChatRoomModel>.Filter.ElemMatch(
                cr => cr.Participant,
                p => p.UserName == userName
            );

            // Lấy tất cả các phòng chat phù hợp với bộ lọc, sắp xếp theo LastInteractionTime
            var chatRooms = await _chatRooms
                .Find(filter)
                .SortByDescending(cr => cr.LastInteractionTime)
                .ToListAsync();

            // Lấy danh sách các Id của phòng chat
            var response = chatRooms.Select(cr => cr.Id).ToList();

            return response;
        }
    }
}
