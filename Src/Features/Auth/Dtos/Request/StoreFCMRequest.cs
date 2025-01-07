namespace HUBT_Social_API.Src.Features.Auth.Dtos.Request
{
    public class StoreFCMRequest
    {
        public string FcmToken { get; set; } = string.Empty;
        public string UserID { get; set; } = string.Empty;
        public string DeviceID { get; set; } = string.Empty;
    }
}
