using System.Text;
using HUBT_Social_API.Core.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace HUBT_Social_API.Core.Configurations;

public static class JwtConfiguration
{
    public static IServiceCollection ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSetting>();
        services.Configure<JwtSetting>(configuration.GetSection("JwtSettings"));

        _ = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
#pragma warning disable CS8602
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                };
#pragma warning restore CS8602
               // Cấu hình cho SignalR
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        // Xác định token từ query string cho các request đến SignalR
                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/chathub"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            })
            .AddCookie(IdentityConstants.ApplicationScheme);
            

        return services;
    }
}