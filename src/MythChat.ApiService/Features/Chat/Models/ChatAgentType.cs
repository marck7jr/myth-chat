namespace MythChat.ApiService.Features.Chat.Models;

public record ChatAgentType
{
    public string Name { get; set; } = string.Empty;
    public IEnumerable<ChatAgent> Agents { get; set; } = [];
}
