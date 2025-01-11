namespace HUBTSOCIAL.Src.Features.Chat.Models;


public class Participant
{
    /// <summary>
    /// UserName dùng để định danh người dùng.
    /// </summary>
    public string UserName { get; set; } = string.Empty; // Id của người dùng
    public ParticipantRole Role { get; set; } = ParticipantRole.Member; // Vai trò của người dùng (vd: Admin, Member)
    public string NickName { get; set; } = string.Empty;
}