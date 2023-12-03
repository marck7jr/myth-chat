using Microsoft.Extensions.Options;

namespace MythChat.ApiService.Configuration;

public class ChatOptionsBuilder(IConfiguration configuration) : IConfigureOptions<ChatOptions>
{
    public void Configure(ChatOptions options)
    {
        configuration.GetSection(ChatOptions.Name).Bind(options);
    }
}
