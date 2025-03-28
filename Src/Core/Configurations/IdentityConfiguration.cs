using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using HUBT_Social_API.Features.Auth.Models;
using HUBTSOCIAL.Src.Features.Chat.Helpers;
using Microsoft.AspNetCore.Identity;

namespace HUBT_Social_API.Core.Configurations;

public static class IdentityConfiguration
{
    public static IServiceCollection ConfigureIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        var mongodbConfig = new MongoDbIdentityConfiguration
        {
            MongoDbSettings = new MongoDbSettings
            {
                ConnectionString = configuration.GetSection("ConnectionStrings:AuthService").Get<string>(),
                DatabaseName = "HUBT_Management"
            },
            IdentityOptionsAction = option =>
            {
                option.Password.RequireDigit = true;
                option.Password.RequiredLength = 8;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequireLowercase = true;
                option.Password.RequireUppercase = true;

                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                option.Lockout.MaxFailedAccessAttempts = 5;

                option.User.RequireUniqueEmail = true;
            }
        };

        services.ConfigureMongoDbIdentity<AUser, ARole, Guid>(mongodbConfig)
            .AddUserManager<UserManager<AUser>>()
            .AddSignInManager<SignInManager<AUser>>()
            .AddRoleManager<RoleManager<ARole>>()
            .AddDefaultTokenProviders();
       

        return services;
    }
}