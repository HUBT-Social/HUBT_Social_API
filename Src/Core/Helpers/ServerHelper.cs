using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HUBT_Social_API.Core.Settings;
using Newtonsoft.Json;

namespace HUBT_Social_API.Src.Core.Helpers
{
    public static class ServerHelper
    {
        public static readonly HttpClient client = new HttpClient();
        public static string? GetIPAddress(HttpContext context)
        {
            string forwardedFor = context.Request.Headers["X-Forwarded-For"].ToString();

            if (!string.IsNullOrEmpty(forwardedFor))
            {
                string ip = forwardedFor.Split(',')[0].Trim();
                return ip;
            }

            IPAddress? remoteIp = context.Connection.RemoteIpAddress;

            if (remoteIp != null)
            {
                if (remoteIp.IsIPv4MappedToIPv6)
                {
                    remoteIp = remoteIp.MapToIPv4();
                }
                if (IPAddress.IsLoopback(remoteIp))
                {
                    remoteIp = IPAddress.Parse("192.168.1.1");
                }
                return remoteIp.ToString();
            }

            return null;
        }
        public static async Task<string> GetLocationFromIpAsync(string ipAddress)
        {
            string apiKey = "3810e9eac14f17"; // Thay YOUR_API_KEY bằng API key của bạn
            string url = $"http://ipinfo.io/{ipAddress}/json?token={apiKey}";

            try
            {
                var response = await client.GetStringAsync(url);
                var locationData = JsonConvert.DeserializeObject<LocationResponse>(response);

                // Trả về thông tin về thành phố, khu vực (region) và quốc gia
                return $"{locationData.City},<br> {locationData.Region}, {locationData.Country}";
            }
            catch (Exception)
            {
                return $"";
            }
        }
        public static string ConvertToCustomString(DateTime dateTime)
        {
            // Định dạng chuyển đổi thời gian thành chuỗi
            string formattedDateTime = dateTime.ToString("dddd, dd MMMM yyyy 'at' hh:mm:ss tt");
            return formattedDateTime;
        }

    }
}
