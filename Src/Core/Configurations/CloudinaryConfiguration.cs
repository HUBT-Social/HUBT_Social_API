using CloudinaryDotNet;
using HUBT_Social_API.Core.Service.Upload;

namespace HUBT_Social_API.Core.Configurations;

public static class CloudinaryConfiguration
{
    public static IServiceCollection ConfigureCloudinary(this IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.GetSection("Cloudinary");
        var account = new Account
        (
            config["CloudName"],
            config["ApiKey"],
            config["ApiSecret"]
        );
        var cloudinary = new Cloudinary(account);

        UploadToStoreS3.CloudinaryService.ConfigureCloudinary(cloudinary); // Cấu hình CloudinaryService


        return services;
    }
}