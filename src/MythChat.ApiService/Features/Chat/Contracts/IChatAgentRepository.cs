using MythChat.ApiService.Features.Chat.Models;

namespace MythChat.ApiService.Features.Chat.Contracts;

public interface IChatAgentRepository
{
    public IEnumerable<ChatAgent> Agents { get; }

    IEnumerable<ChatAgent> GetAgents(string? query);
    ChatAgent? GetAgent(string? name, string? group);
    IEnumerable<ChatAgentGroup> GetGroups(string? query);
}
