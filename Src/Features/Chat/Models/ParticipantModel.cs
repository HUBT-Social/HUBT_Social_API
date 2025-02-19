using System.Threading.Tasks;
using Amazon.Runtime;
using FirebaseAdmin.Auth;
using HUBT_Social_API.Core.Settings;
using HUBTSOCIAL.Src.Features.Chat.Collections;
using HUBTSOCIAL.Src.Features.Chat.Helpers;

namespace HUBTSOCIAL.Src.Features.Chat.Models;


public class Participant
{
    /// <summary>
    /// UserName dùng để định danh người dùng.
    /// </summary>
    public string UserId { get; set; } = string.Empty; // Id của người dùng
    public ParticipantRole Role { get; set; } // Vai trò của người dùng (vd: Admin, Member)
    public string NickName { get; set; } = string.Empty;
    public string? ProfilePhoto { get; set; }

    /// <summary>
    /// Ảnh đại diện mặc định nếu ProfilePhoto không có.
    /// </summary>
    public string DefaultAvatarImage { get; set; } = LocalValue.Get(KeyStore.DefaultUserImage);

    public Participant(UserHelper userHelper, string userId,ParticipantRole? role)
    {
        this.UserId = userId;
        this.NickName = userHelper.GetFullNameById(userId).ToString();
        this.ProfilePhoto = userHelper.GetAvaterUserById(userId).ToString();
        this.Role = role??ParticipantRole.Member;
    }
   
}