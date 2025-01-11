namespace HUBT_Social_API.Features.Chat.DTOs;


public class MessageInputRequest {
    public string GroupId { get; set; } = string.Empty;
    public string? Content { get; set; } =string.Empty;
}
public class MediaInputRequest
{
    public string GroupId { get; set; } = string.Empty;
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


