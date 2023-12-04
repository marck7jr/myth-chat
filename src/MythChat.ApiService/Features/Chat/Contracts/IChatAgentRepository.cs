using MythChat.ApiService.Features.Chat.Models;

namespace MythChat.ApiService.Features.Chat.Contracts;

public interface IChatAgentRepository
{
    public IEnumerable<ChatAgent> Agents { get; }

    IEnumerable<ChatAgent> GetAgents(string? query = null);
    IEnumerable<ChatAgentGroup> GetGroups(string? query = null);
    IEnumerable<ChatAgentType> GetTypes(string? query = null);
}
