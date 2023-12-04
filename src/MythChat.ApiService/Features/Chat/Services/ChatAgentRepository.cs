using Mapster;

using Microsoft.Extensions.Options;

using MythChat.ApiService.Configuration;
using MythChat.ApiService.Features.Chat.Contracts;
using MythChat.ApiService.Features.Chat.Models;

namespace MythChat.ApiService.Features.Chat.Services;

public class ChatAgentRepository(
    IOptionsSnapshot<ChatOptions> options) : IChatAgentRepository
{
    private readonly IReadOnlyList<ChatAgent> _agents = options.Value.Agents.Adapt<IReadOnlyList<ChatAgent>>();

    public IEnumerable<ChatAgent> Agents { get => _agents; }

    public IEnumerable<ChatAgent> GetAgents(string? query = null)
    {
        var agents = !string.IsNullOrWhiteSpace(query)
            ? Agents.Where(x =>
                x.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Group.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Type.Contains(query, StringComparison.OrdinalIgnoreCase))
            : Agents;

        return agents ?? [];
    }

    public IEnumerable<ChatAgentGroup> GetGroups(string? query = null)
    {
        var agents = !string.IsNullOrWhiteSpace(query)
            ? Agents.Where(x => x.Group.Contains(query, StringComparison.OrdinalIgnoreCase))
            : Agents;

        var groups = agents
            .GroupBy(x => x.Group)
            .Select(group => new ChatAgentGroup
            {
                Name = group.Key,
                Agents = group
            });

        return groups ?? [];
    }

    public IEnumerable<ChatAgentType> GetTypes(string? query = null)
    {
        var agents = !string.IsNullOrWhiteSpace(query)
            ? Agents.Where(x => x.Type.Contains(query, StringComparison.OrdinalIgnoreCase))
            : Agents;

        var types = agents
            .GroupBy(x => x.Type)
            .Select(type => new ChatAgentType
            {
                Name = type.Key,
                Agents = type
            });

        return types ?? [];
    }
}
