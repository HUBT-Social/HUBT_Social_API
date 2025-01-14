using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.IO;

namespace HUBT_Social_API.Core.Configurations;

public static class FirebaseConfiguration
{
    public static IServiceCollection FirebaseService(this IServiceCollection services, IConfiguration config)
    {


        string? BasePath = config.GetSection("FirebaseSetting:Credential").Get<string>();
    
        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromFile(BasePath),
        });
        return services;
    }
}