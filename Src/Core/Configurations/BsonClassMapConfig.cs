using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Bson.Serialization;

public static class BsonClassMapConfig
{
    public static IServiceCollection RegisterClassMaps(this IServiceCollection services)
    {
        // Đăng ký lớp gốc ChatItem
        BsonClassMap.RegisterClassMap<ChatItem>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true); // Định nghĩa đây là lớp gốc
            cm.MapMember(c => c.Type).SetElementName("Type");
        });

        // Đăng ký lớp con MessageChatItem
        BsonClassMap.RegisterClassMap<MessageChatItem>(cm =>
        {
            cm.AutoMap();
        });

        // Đăng ký lớp con MediaChatItem
        BsonClassMap.RegisterClassMap<MediaChatItem>(cm =>
        {
            cm.AutoMap();
        });
         // Đăng ký lớp con MediaChatItem
        BsonClassMap.RegisterClassMap<FileChatItem>(cm =>
        {
            cm.AutoMap();
        });
         // Đăng ký lớp con MediaChatItem
        BsonClassMap.RegisterClassMap<VoiceChatItem>(cm =>
        {
            cm.AutoMap();
        });
        return services;
    }
}
