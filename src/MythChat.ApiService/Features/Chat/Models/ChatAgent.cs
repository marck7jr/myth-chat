namespace MythChat.ApiService.Features.Chat.Models;

public class ChatAgent
{
    public string Description { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public Dictionary<string, object?> Metadata { get; set; } = [];
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}
