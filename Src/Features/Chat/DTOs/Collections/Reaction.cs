
using HUBTSOCIAL.Src.Features.Chat.Collections;
using HUBTSOCIAL.Src.Features.Chat.Models;

public class Reaction 
{
    public string UserName { get; set; } = string.Empty;
    public ReactionDetail ReactionCollection { get; set; }
}