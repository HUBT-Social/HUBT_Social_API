namespace HUBT_Social_API.Features.Chat.DTOs;
public class UpdateNickNameRequest
{
    public string GroupId { get; set; }
    public string UserName { get; set; }
    public string NewNickName { get; set; }
}
public class UpdateGroupNameRequest
{
    public string Id { get; set; }
    public string NewName { get; set; }
}
public class UpdateAvatarRequest
{
    public string Id { get; set; }
    public FormFile file { get; set; }
}
public class AddMemberRequest
{
    public string GroupId { get; set; }
    public string UserName { get; set; }
}

public class RemoveMemberRequest
{
    public string GroupId { get; set; }
    public string UserName { get; set; }
}

public class LeaveRoomRequest
{
    public string GroupId { get; set; }
}
    