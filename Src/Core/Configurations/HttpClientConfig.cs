using HUBT_Social_API.Src.Features.Notifcate.Services;
using HUBT_Social_Base.Service;
using Notation_API.Src.Services;
using System.ComponentModel.Design;

namespace HUBT_Social_API.Src.Core.Configurations
{
    public static class HttpClientConfig
    {
        public static IServiceCollection AddRegisterClientService(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            string? HubtPath = configuration.GetSection("HUBT_Data").Get<string>();
            services.AddHttpContextAccessor();
            services.AddHttpClient();
            services.AddScoped<IHttpService, HttpService>();
            services.AddScoped<INotationService>(sp =>
            {
                var httpService = sp.GetRequiredService<IHttpService>();
                return ActivatorUtilities.CreateInstance<NotationService>(sp, httpService, HubtPath);
            });
            return services;
        }

    }
}
