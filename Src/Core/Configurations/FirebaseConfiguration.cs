using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HUBT_Social_API.Core.Configurations;

public static class FirebaseConfiguration
{
    public static IServiceCollection FirebaseService(this IServiceCollection services, IConfiguration config)
    {
        var firebaseCredentialSection = config.GetSection("FirebaseSetting:Credential");
        var firebaseCredentialJson = firebaseCredentialSection.GetChildren().ToDictionary(x => x.Key, x => x.Value);
        var firebaseCredentialString = JsonConvert.SerializeObject(firebaseCredentialJson);

        if (string.IsNullOrEmpty(firebaseCredentialString))
        {
            throw new InvalidOperationException("Firebase credentials are missing or invalid in the configuration.");
        }

        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromJson(firebaseCredentialString)
        });
        return services;
    }
}