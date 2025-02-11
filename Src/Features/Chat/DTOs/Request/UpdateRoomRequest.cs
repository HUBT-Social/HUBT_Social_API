namespace HUBT_Social_API.Features.Chat.DTOs;

public class UpdateNickNameRequest
{
    public string GroupId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string NewNickName { get; set; } = string.Empty;
}

public class UpdateGroupNameRequest
{
    public string Id { get; set; } = string.Empty;
    public string NewName { get; set; } = string.Empty;
}

public class UpdateAvatarRequest
{
    public string Id { get; set; } = string.Empty;
    public FormFile file { get; set; }
}

public class AddMemberRequest
{
    public string GroupId { get; set; } = string.Empty;
    public string AddedName { get; set; } = string.Empty;
}

public class RemoveMemberRequest
{
    public string GroupId { get; set; } = string.Empty;
    public string KickedName { get; set; } = string.Empty;
}

public class LeaveRoomRequest
{
    public string GroupId { get; set; } = string.Empty;
}