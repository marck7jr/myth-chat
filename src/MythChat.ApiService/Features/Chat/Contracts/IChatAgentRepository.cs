using MythChat.ApiService.Features.Chat.Models;

namespace MythChat.ApiService.Features.Chat.Contracts;

public interface IChatAgentRepository
{
    public IEnumerable<ChatAgent> Agents { get; }

    ChatAgent? GetAgent(string? query);
    ChatAgent? GetAgentByNameAndGroup(string? name, string? group);
    IEnumerable<ChatAgent> GetAgentsByGroup(string? group);
    IEnumerable<ChatAgent> GetAgentsByType(string? type);
}
