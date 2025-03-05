using System.Net;

namespace HUBT_Social_API.Src.Features.Notifcate.Models.Requests
{
    public class ResponseDTO
    {
        public object? Data { get; set; }
        public string Message { get; set; } = string.Empty;

        public HttpStatusCode StatusCode { get; set; }
    }
}
