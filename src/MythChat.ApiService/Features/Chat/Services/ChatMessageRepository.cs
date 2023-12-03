using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

using MythChat.ApiService.Features.Chat.Contracts;
using MythChat.ApiService.Features.Chat.Models;

namespace MythChat.ApiService.Features.Chat.Services;

public class ChatMessageRepository(
    IDistributedCache distributedCache,
    ILogger<ChatMessageRepository> logger) : IChatMessageRepository
{
    public async Task<IEnumerable<string>> GetMessagesAsync(ChatAgent? agent, string? channel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(agent);
        ArgumentException.ThrowIfNullOrWhiteSpace(channel);

        var cacheKey = $"{agent.Group}:{agent?.Name}:{channel}";
        var cachedContext = await distributedCache.GetStringAsync(cacheKey, cancellationToken);

        var history = cachedContext is not null
            ? JsonSerializer.Deserialize<List<string>>(cachedContext) ?? []
            : [];

        logger.LogInformation("Retrieved {Count} messages from {CacheKey}", history.Count, cacheKey);

        return history ?? [];
    }

    public async Task<IEnumerable<string>> SaveMessagesAsync(ChatAgent? agent, string? channel, IEnumerable<string>? messages, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(agent);
        ArgumentException.ThrowIfNullOrWhiteSpace(channel);
        ArgumentNullException.ThrowIfNull(messages);

        var cacheKey = $"{agent.Group}:{agent?.Name}:{channel}";
        var cachedContext = await distributedCache.GetStringAsync(cacheKey, cancellationToken);

        var history = cachedContext is not null
            ? JsonSerializer.Deserialize<List<string>>(cachedContext) ?? []
        : [];

        history.AddRange(messages);

        await distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(history), cancellationToken);

        logger.LogInformation("Saved {Count} messages to {CacheKey}", history.Count, cacheKey);

        return history;
    }

}
