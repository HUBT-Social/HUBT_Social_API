using HUBT_Social_API.Core.Settings;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Core.Configurations;

public static class ConfigureService
{
    public static IServiceCollection AddConfigureationService(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<JwtSetting>(config.GetSection("JwtSettings"));
        services.Configure<SMPTSetting>(config.GetSection("SMPTSetting"));
        services.Configure<ApiBehaviorOptions>(option => { option.SuppressModelStateInvalidFilter = true; });
        return services;
    }
}