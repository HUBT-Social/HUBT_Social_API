using HUBTSOCIAL.Src.Features.Chat.Collections;

public class Reaction
{
    public List<string> Reactions { get; set; } = new List<string>();
    public List<string> reactedUserIds { get; set; } = new List<string>();
    
}