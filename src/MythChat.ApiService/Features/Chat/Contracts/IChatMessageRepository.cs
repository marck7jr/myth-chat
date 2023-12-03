using MythChat.ApiService.Features.Chat.Models;

namespace MythChat.ApiService.Features.Chat.Contracts;

public interface IChatMessageRepository
{
    Task<IEnumerable<ChatMessage>> GetMessagesAsync(ChatAgent? agent, string? channel, CancellationToken cancellationToken = default);
    Task<IEnumerable<ChatMessage>> SaveMessagesAsync(ChatAgent? agent, string? channel, IEnumerable<ChatMessage>? messages, CancellationToken cancellationToken = default);
}
