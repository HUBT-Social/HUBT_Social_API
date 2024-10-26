using Microsoft.Extensions.DependencyInjection;

namespace HUBT_Social_API.src.Core.Configurations
{
    public static class SignalRConfiguration
    {
        public static IServiceCollection ConfigureSignalR(this IServiceCollection services)
        {
            // Đăng ký SignalR
            services.AddSignalR();
            return services;
        }
    }
}
