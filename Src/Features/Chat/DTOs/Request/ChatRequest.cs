namespace HUBT_Social_API.Features.Chat.DTOs;

public enum StatusSending
{
    Sending,
    Sent,
    SendingFaild
}
public class ChatRequestBase
{
    public string GroupId { get; set; } = string.Empty;

}
public class MessageInputRequest : ChatRequestBase
{
    public string? Content { get; set; } =string.Empty;
}
public class MediaInputRequest : ChatRequestBase
{
    public List<IFormFile>? Files { get; set; } = [];
}




public class MessageRequest : MessageInputRequest
{
    public string UserName { get; set; } = string.Empty;
    
}
public class MediaRequest : MediaInputRequest
{
    public string? UserName { get; set; } = string.Empty;
}


public class DeleteGroupRequest
    {
        public string GroupId { get; set; }
    }
public class GetGroupByIdRequest
    {
        public string GroupId { get; set; }
    }
public class SearchGroupsRequest
    {
        public string Keyword { get; set; }
    }

public class GetRoomsByUserRequest
    {
        public string UserName { get; set; }
    }


