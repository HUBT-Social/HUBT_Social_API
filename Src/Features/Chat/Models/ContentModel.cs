namespace HUBTSOCIAL.Src.Features.Chat.Models;

public class ContentModel
{
    public string? Message { get; set; } = string.Empty;
    public List<string>? Images { get; set; } = []; 
    public string? Files { get; set; } = string.Empty;
 
}

