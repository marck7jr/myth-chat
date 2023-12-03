using System.Text.Json;

using Mapster;

using Microsoft.Extensions.Caching.Distributed;
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

    public ChatAgent? GetAgent(string? query)
    {
        ArgumentNullException.ThrowIfNull(query);

        var agent = Agents.FirstOrDefault(x =>
            x.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            x.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            x.Group.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            x.Type.Contains(query, StringComparison.OrdinalIgnoreCase));

        return agent;
    }

    public IEnumerable<ChatAgent> GetAgentsByGroup(string? group)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(group);

        var agents = Agents
            .Where(x => string.Equals(x.Group, group, StringComparison.OrdinalIgnoreCase));

        return agents;
    }

    public ChatAgent? GetAgentByNameAndGroup(string? name, string? group)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(group);

        var agent = Agents.FirstOrDefault(x =>
            string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(x.Group, group, StringComparison.OrdinalIgnoreCase));

        return agent;
    }

    public IEnumerable<ChatAgent> GetAgentsByType(string? type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(type);

        var agents = Agents
            .Where(x => string.Equals(x.Type, type, StringComparison.OrdinalIgnoreCase));

        return agents;
    }
}
