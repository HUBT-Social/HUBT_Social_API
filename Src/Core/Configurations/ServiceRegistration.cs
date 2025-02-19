using HUBT_Social_API.Features.Auth.Services;
using HUBT_Social_API.Features.Auth.Services.Child;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.Features.Chat.ChatHubs;
using HUBT_Social_API.Features.Chat.Services;
using HUBT_Social_API.Features.Chat.Services.Child;
using HUBT_Social_API.Features.Chat.Services.Interfaces;
using HUBT_Social_API.Src.Features.Notifcate.Services;
using HUBTSOCIAL.Src.Features.Chat.Helpers;

namespace HUBT_Social_API.Core.Configurations;

public static class ServiceRegistration
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        // Đăng ký các dịch vụ
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUploadChatServices, UploadChatServices>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<IRegisterService, RegisterService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<ChatHub>();
        services.AddScoped<IMessageUploadService, MessageUploadService>();
        services.AddScoped<IMediaUploadService, MediaUploadService>();
        services.AddScoped<IFileUploadService, FileUploadService>();
        services.AddScoped<IUploadChatServices, UploadChatServices>();
        services.AddSingleton<IUserConnectionManager, UserConnectionManager>();
        services.AddScoped<IFireBaseNotificationService, FireBaseNotificationService>();
        services.AddScoped<UserHelper>();

        /*            services.AddScoped<IUserManagerS, UserManagerS>();
        */

        return services;
    }
}