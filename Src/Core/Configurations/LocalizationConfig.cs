using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

namespace HUBT_Social_API.Core.Configurations;

public static class LocalizationConfig
{
    public static IServiceCollection ConfigureLocalization(this IServiceCollection services)
    {
        // Đăng ký localization và các ngôn ngữ được hỗ trợ
        services.AddLocalization(options => options.ResourcesPath = "Resources");

        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[]
            {
                new CultureInfo("vi"),
                new CultureInfo("en")
                //  Thêm các ngôn ngữ khác ở đây
            };
            const string defaultCulture = "en";

            options.DefaultRequestCulture = new RequestCulture(defaultCulture);
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures; // Các UI cultures hỗ trợ

            // Chọn ngôn ngữ dựa trên query string (https://ip/login?culture=en), cookie hoặc header 'Accept-Language'
            options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
        });
        return services;
    }

    public static IApplicationBuilder UseLocalization(this IApplicationBuilder app)
    {
        // Áp dụng middleware localization
        var options = app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
        app.UseRequestLocalization(options);
        return app;
    }
}