using HUBTSOCIAL.Src.Features.Chat.Collections;

public class Reaction
{
    public string UserName { get; set; } = string.Empty;
    public ReactionDetail ReactionCollection { get; set; }
}