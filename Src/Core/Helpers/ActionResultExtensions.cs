using HUBT_Social_API.Src.Features.Notifcate.Models.Requests;
using Newtonsoft.Json;

namespace HUBT_Social_API.Src.Core.Helpers
{
    public static class ActionResultExtensions
    {
        public static T? ConvertTo<T>(this ResponseDTO result) where T : class
        {
            if (result != null)
            {
                var value = result.Data;

                try
                {
                    var json = JsonConvert.SerializeObject(value);
                    return JsonConvert.DeserializeObject<T>(json);
                }
                catch (JsonException)
                {
                    return null;
                }

            }
            return null;
        }
    }
}
