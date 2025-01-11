using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.src.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Src.Features.Auth.Dtos.Collections;
using HUBTSOCIAL.Src.Features.Chat.Helpers;
using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Driver;

namespace HUBT_Social_API.Core.Configurations;

public static class MongoDbConfiguration
{
    public static IServiceCollection AddAuthMongoCollections(this IServiceCollection services,
        IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetConnectionString("AuthService"));
        var database = client.GetDatabase("HUBT_Management");

        services.AddScoped<IMongoCollection<UserToken>>(s => database.GetCollection<UserToken>("RefreshTokens"));
        services.AddScoped<IMongoCollection<Postcode>>(s => database.GetCollection<Postcode>("Postcode"));
        services.AddScoped<IMongoCollection<PromotedStore>>(s => database.GetCollection<PromotedStore>("UserFCMToken"));
        services.AddScoped<IMongoCollection<TempUserRegister>>(s =>
            database.GetCollection<TempUserRegister>("TempUserRegister"));

        return services;
    }

    public static IServiceCollection AddChatMongoCollections(this IServiceCollection services,
        IConfiguration configuration)
    {
        var chatClient = new MongoClient(configuration.GetConnectionString("ChatService"));
        var chatDatabase = chatClient.GetDatabase("HUBT_Management");

        services.AddScoped<IMongoCollection<ChatRoomModel>>(s => chatDatabase.GetCollection<ChatRoomModel>("ChatRoom"));
        RoomChatHelper.Initialize(chatDatabase.GetCollection<ChatRoomModel>("ChatRoom"));
        return services;
    }
}