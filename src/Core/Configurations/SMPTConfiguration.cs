using HUBT_Social_API.src.Core.Settings;
using Microsoft.Extensions.DependencyInjection;


namespace HUBT_Social_API.src.Core.Configurations
{
    public static class SMPTConfiguration
    {
        public static IServiceCollection ConfigureSMPT(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SMPTSetting>(configuration.GetSection("SMPTSetting"));
            return services;
        }
    }
}