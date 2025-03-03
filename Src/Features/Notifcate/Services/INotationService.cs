
using HUBT_Social_API.Src.Features.Notifcate.Models.OutSourceDataDTO;
using HUBT_Social_API.Src.Features.Notifcate.Services;

namespace Notation_API.Src.Services
{
    public interface INotationService : IBaseService
    {
        Task<StudentDTO?> GetStudentByMasv(string masv);
        Task<List<TimeTableDTO>?> GetTimeTableByClassName(string className);
        Task<AVGScoreDTO?> GetAVGScoreByMasv(string masv);
        Task<List<ScoreDTO>?> GetStudentScoreByMasv(string masv);


    }
}
