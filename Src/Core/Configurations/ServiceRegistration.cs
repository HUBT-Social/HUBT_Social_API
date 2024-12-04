using HUBT_Social_API.Features.Auth.Services;
using HUBT_Social_API.Features.Auth.Services.Child;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using HUBT_Social_API.Features.Chat.ChatHubs.ChildChatHubs;
using HUBT_Social_API.Features.Chat.ChatHubs.IHubs;
using HUBT_Social_API.Features.Chat.Services;
using HUBT_Social_API.Features.Chat.Services.Child;
using HUBT_Social_API.Features.Chat.Services.Interfaces;

namespace HUBT_Social_API.Core.Configurations;

public static class ServiceRegistration
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        // Đăng ký các dịch vụ
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IEmailService, EmailService>();
        services.AddSingleton<ITokenService, TokenService>();
        services.AddSingleton<IChatService, ChatService>();
        services.AddSingleton<IUploadChatServices, UploadChatServices>();
        services.AddSingleton<IChatRoomService, ChatRoomService>();
        services.AddSingleton<IChatMessageHub, ChatMessageHub>();
        services.AddSingleton<IChatFileHub, ChatFileHub>();
        services.AddSingleton<ILoginService, LoginService>();
        services.AddSingleton<IRegisterService, RegisterService>();
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IChatRoomService, ChatRoomService>();


/*            services.AddScoped<IUserManagerS, UserManagerS>();
*/

        return services;
    }
}