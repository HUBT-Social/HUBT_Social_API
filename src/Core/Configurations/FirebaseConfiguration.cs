using FireSharp.Config;
using FireSharp.Interfaces;
using HUBT_Social_API.src.Core.Settings;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;

namespace HUBT_Social_API.src.Core.Configurations
{
    public static class FirebaseConfiguration
    {
        public static IServiceCollection FirebaseService(this IServiceCollection services, IConfiguration config)
        {


            services.AddSingleton<IFirebaseConfig>(s => new FirebaseConfig
            {
                AuthSecret = config.GetValue<string>("FirebaseSetting:AuthSecret"),
                BasePath = config.GetValue<string>("FirebaseSetting:BasePath")
            });

            return services;
        }

    }
}
