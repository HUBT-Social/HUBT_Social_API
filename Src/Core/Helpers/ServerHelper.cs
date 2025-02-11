using System.Net;
using HUBT_Social_API.Core.Settings;
using Newtonsoft.Json;

namespace HUBT_Social_API.Src.Core.Helpers;

public static class ServerHelper
{
    public static readonly HttpClient client = new();

    public static string? GetIPAddress(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].ToString();

        if (!string.IsNullOrEmpty(forwardedFor))
        {
            var ip = forwardedFor.Split(',')[0].Trim();
            return ip;
        }

        var remoteIp = context.Connection.RemoteIpAddress;

        if (remoteIp != null)
        {
            if (remoteIp.IsIPv4MappedToIPv6) remoteIp = remoteIp.MapToIPv4();
            if (IPAddress.IsLoopback(remoteIp)) remoteIp = IPAddress.Parse("192.168.1.1");
            return remoteIp.ToString();
        }

        return null;
    }

    public static async Task<string> GetLocationFromIpAsync(string ipAddress)
    {
        var apiKey = "3810e9eac14f17"; // Thay YOUR_API_KEY bằng API key của bạn
        var url = $"http://ipinfo.io/{ipAddress}/json?token={apiKey}";

        try
        {
            var response = await client.GetStringAsync(url);
            var locationData = JsonConvert.DeserializeObject<LocationResponse>(response);

            // Trả về thông tin về thành phố, khu vực (region) và quốc gia
            return $"{locationData.City},<br> {locationData.Region}, {locationData.Country}";
        }
        catch (Exception)
        {
            return "";
        }
    }

    public static string ConvertToCustomString(DateTime dateTime)
    {
        // Định dạng chuyển đổi thời gian thành chuỗi
        var formattedDateTime = dateTime.ToString("dddd, dd MMMM yyyy 'at' hh:mm:ss tt");
        return formattedDateTime;
    }
}