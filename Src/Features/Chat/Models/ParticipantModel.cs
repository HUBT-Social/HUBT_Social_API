using HUBT_Social_API.Core.Settings;
using HUBTSOCIAL.Src.Features.Chat.Collections;
using HUBTSOCIAL.Src.Features.Chat.Helpers;

namespace HUBTSOCIAL.Src.Features.Chat.Models;


public class Participant
{
    /// <summary>
    /// UserName dùng để định danh người dùng.
    /// </summary>
    public string UserName { get; set; } = string.Empty; // Id của người dùng
    public ParticipantRole Role { get; set; } = ParticipantRole.Member; // Vai trò của người dùng (vd: Admin, Member)
    public string NickName { get; set; } = string.Empty;
    public string? ProfilePhoto { get; set; }

    /// <summary>
    /// Ảnh đại diện mặc định nếu ProfilePhoto không có.
    /// </summary>
    public string DefaultAvatarImage { get; set; } = LocalValue.Get(KeyStore.DefaultUserImage);
   

    public void setProfilePhoto(string ProfilePhoto)
    {
        this.ProfilePhoto = ProfilePhoto;
    }
    

}