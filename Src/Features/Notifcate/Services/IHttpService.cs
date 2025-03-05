using HUBT_Social_API.Src.Features.Notifcate.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Src.Features.Notifcate.Services
{
    public interface IHttpService
    {
        Task<ResponseDTO> SendAsync(RequestDTO request);
    }
}
