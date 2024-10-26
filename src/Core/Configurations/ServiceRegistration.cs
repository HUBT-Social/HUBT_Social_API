using Microsoft.Extensions.DependencyInjection;
using HUBTSOCIAL.Src.Features.Auth.Services;
using HUBTSOCIAL.Src.Features.Chat.Services;
using HUBTSOCIAL.Src.Features.Chat.Services.IChatServices;
using HUBTSOCIAL.Src.Features.Chat.Services.ChildChatServices;
using HUBTSOCIAL.Src.Features.Chat.Hubs.ChildChatHubs;
using HUBTSOCIAL.Src.Features.Chat.ChatHubs.IHubs;
using HUBT_Social_API.src.Features.Login.Services;

using HUBT_Social_API.src.Features.Auth.Services.IAuthServices;
using HUBT_Social_API.src.Features.Auth.Services.ChildServices;
using HUBT_Social_API.src.Features.Auth.Services;

namespace HUBT_Social_API.src.Core.Configurations
{
    public static class ServiceRegistration
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
        {
            // Đăng ký các dịch vụ
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IChatMessageHub, ChatMessageHub>();
            services.AddScoped<IChatFileHub, ChatFileHub>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IRegisterService, RegisterService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<RegisterService>();
            services.AddScoped<IChatRoomService, ChatRoomService>();

/*            services.AddScoped<IUserManagerS, UserManagerS>();
*/            services.AddScoped<IEmailService, EmailService>();


            return services;
        }
    }
}
