using System.Net;


namespace HUBT_Social_API.Src.Core.Helpers
{
    public static class ServerHelper
    {
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
    }
}
