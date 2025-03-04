using HUBTSOCIAL.Src.Features.Chat.Collections;
using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace HUBTSOCIAL.Src.Features.Chat.Models
{
    public class ChatRoomModel
    {
        [BsonId]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string BackGroundUrl { get; set; } = string.Empty;
        public DateTime LastInteractionTime { get; set; }
        public List<Participant> Participant { get; set; } = new();
        public string PreBlockId { get; set; }= "First_Block";
        public List<PageReference> PageReference { get; set; } = new();
        public List<MessageModel> HotContent { get; set; } = new(); // Chỉ là danh sách, logic tách riêng
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public TypeChatRoom TypeChatRoom { get; set; } = TypeChatRoom.None;

    }

}
