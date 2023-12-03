
namespace MythChat.ApiService.Features.Chat.Models;

public class ChatMessage(string? channel, string? author, string? message)
{
    public string Author { get; set; } = author ?? throw new ArgumentNullException(nameof(author));
    public string Channel { get; set; } = channel ?? throw new ArgumentNullException(nameof(channel));
    public string Message { get; set; } = message ?? throw new ArgumentNullException(nameof(message));

    public override string ToString()
    {
        return $"{Author}: {Message}";
    }
}
