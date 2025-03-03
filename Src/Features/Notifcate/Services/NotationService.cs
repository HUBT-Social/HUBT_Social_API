using HUBT_Social_API.Src.Features.Notifcate.Models.Requests;
using HUBT_Social_API.Src.Features.Notifcate.Services;
using HUBT_Social_API.Src.Core.Helpers;
using HUBT_Social_API.Src.Features.Notifcate.Models.OutSourceDataDTO;
using HUBT_Social_API.Core.Settings;

namespace Notation_API.Src.Services
{
    public class NotationService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), INotationService
    {
        public async Task<AVGScoreDTO?> GetAVGScoreByMasv(string masv)
        {
            string path = $"diemtb?masv={masv}";
            ResponseDTO response = await SendRequestAsync(path, ApiType.GET);
            return response.ConvertTo<AVGScoreDTO>();
        }

        public async Task<StudentDTO?> GetStudentByMasv(string masv)
        {
            string path = $"sinhvien?masv={masv}";
            ResponseDTO response = await SendRequestAsync(path, ApiType.GET);
            return response.ConvertTo<StudentDTO>();
        }

        public async Task<List<ScoreDTO>?> GetStudentScoreByMasv(string masv)
        {
            string path = $"sinhvien/{masv}/diem";
            ResponseDTO response = await SendRequestAsync(path, ApiType.GET);
            return response.ConvertTo<List<ScoreDTO>>(); ;
        }

        public async Task<List<TimeTableDTO>?> GetTimeTableByClassName(string className)
        {
            string path = $"ThoiKhoaBieu?className={className}";
            ResponseDTO response = await SendRequestAsync(path, ApiType.GET);
            return response.ConvertTo<List<TimeTableDTO>>();
        }
    }
}
