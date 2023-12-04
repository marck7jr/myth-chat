namespace MythChat.ApiService.Features.Chat.Models;

public record ChatAgent
{
    public string Description { get; init; } = string.Empty;
    public string Group { get; init; } = string.Empty;
    public Dictionary<string, object?> Metadata { get; init; } = [];
    public string Name { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
}
