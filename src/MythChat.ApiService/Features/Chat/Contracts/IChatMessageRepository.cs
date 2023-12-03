using MythChat.ApiService.Features.Chat.Models;

namespace MythChat.ApiService.Features.Chat.Contracts;

public interface IChatMessageRepository
{
    Task<IEnumerable<string>> GetMessagesAsync(ChatAgent? agent, string? channel, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> SaveMessagesAsync(ChatAgent? agent, string? channel, IEnumerable<string>? messages, CancellationToken cancellationToken = default);
}
