
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Runtime.CompilerServices;
using FireSharp.Interfaces;
using FireSharp.Config;
using HUBT_Social_API.src.Core.Settings;
using HUBT_Social_API.src.Features.Login.Services;

namespace HUBT_Social_API.src.Core.Configurations
{
    public static class ConfigureService
    {
        public static IServiceCollection AddConfigureationService(this IServiceCollection services, IConfiguration config)
        {

            services.Configure<JwtSetting>(config.GetSection("JwtSettings"));
            services.Configure<SMPTSetting>(config.GetSection("SMPTSetting"));


            return services;
        }
    }
}
