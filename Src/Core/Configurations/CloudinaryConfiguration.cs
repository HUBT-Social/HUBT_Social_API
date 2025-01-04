using CloudinaryDotNet;
using HUBT_Social_API.Core.Service.Upload;

namespace HUBT_Social_API.Core.Configurations;

public static class CloudinaryConfiguration
{
    public static IServiceCollection ConfigureCloudinary(this IServiceCollection services, IConfiguration configuration)
    {
      
            IConfigurationSection config = configuration.GetSection("Cloudinary");
            Account account = new Account
            (
                config["CloudName"],
                config["ApiKey"],
                config["ApiSecret"]
            );
            Cloudinary cloudinary = new Cloudinary(account);

            UploadToStoreS3.CloudinaryService.ConfigureCloudinary(cloudinary); // Cấu hình CloudinaryService
            
        
        return services;
    }
}