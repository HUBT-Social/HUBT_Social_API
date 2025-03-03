using HUBT_Social_API.Core.Settings;

namespace HUBT_Social_API.Src.Features.Notifcate.Models.Requests
{
    public class RequestDTO
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; } = string.Empty;
        public object? Data { get; set; }
        public string? AccessToken { get; set; }
    }
}
