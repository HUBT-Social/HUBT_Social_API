namespace HUBT_Social_API.Features.Chat.DTOs;
public class UpdateNickNameRequest
{
    public string GroupId { get; set; } = String.Empty;
    public string UserName { get; set; } = String.Empty;
    public string NewNickName { get; set; } = String.Empty;
}
public class UpdateGroupNameRequest
{
    public string Id { get; set; } = String.Empty;
    public string NewName { get; set; } = String.Empty;
}
public class UpdateAvatarRequest
{
    public string Id { get; set; } = String.Empty;
    public FormFile file { get; set; } 
}
public class AddMemberRequest
{
    public string GroupId { get; set; } = String.Empty;
    public string AddedName { get; set; } = String.Empty;
}

public class RemoveMemberRequest
{
    public string GroupId { get; set; } = String.Empty;
    public string KickedName { get; set; } = String.Empty;
}

public class LeaveRoomRequest
{
    public string GroupId { get; set; } = String.Empty;
}
    