using Amazon.S3;
using B2Net;
using B2Net.Models;
using HUBT_Social_API.Core.Service.Upload;
using Microsoft.Extensions.Options;

namespace HUBT_Social_API.Core.Configurations
{
    public static class BackblazeB2Configuration
    {
        public static IServiceCollection ConfigureBackblazeB2(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetSection("Backblaze").Get<BackblazeB2Options>();

            if (string.IsNullOrEmpty(options.KeyId) || string.IsNullOrEmpty(options.ApplicationKey))
            {
                throw new InvalidOperationException("Key ID or Application Key is missing.");
            }

            var b2Client = new B2Client(new B2Options
            {
                KeyId = options.KeyId,
                ApplicationKey = options.ApplicationKey
            });
 
                // Khởi tạo BackblazeB2Service
                UploadToStoreS3.BackblazeB2Service.InitBackblazeB2Service(b2Client,options.KeyId,options.ApplicationKey, options.BucketId, options.BucketName);
       

            return services;
        }
    }
}
