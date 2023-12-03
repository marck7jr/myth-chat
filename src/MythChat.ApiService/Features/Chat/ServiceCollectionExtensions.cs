using MythChat.ApiService.Features.Chat.Contracts;
using MythChat.ApiService.Features.Chat.Services;

namespace MythChat.ApiService.Features.Chat;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddChatFeature(this IServiceCollection services)
    {
        services.AddScoped<IChatAgentRepository, ChatAgentRepository>();
        services.AddScoped<IChatMessageRepository, ChatMessageRepository>();

        return services;
    }
}
