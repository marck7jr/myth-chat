using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;

using MythChat.ApiService.Configuration;

namespace MythChat.ApiService.Extensions;

public static class SemanticKernelExtensions
{
    public static IServiceCollection AddSemanticKernel(this IServiceCollection services)
    {
        services.AddScoped(provider =>
        {
            var optionsSnapshot = provider.GetRequiredService<IOptionsSnapshot<SemanticKernelOptions>>();
            var options = optionsSnapshot.Value;

            var kernel = new KernelBuilder()
                .WithOpenAIChatCompletionService(options.ModelId, options.SecretKey)
                .Build();

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "sk", "plugins");
            var directories = Directory.GetDirectories(path)
                .Select(x => Path.GetFileName(x))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            var orchestrationPlugin = kernel.ImportSemanticFunctionsFromDirectory(path, pluginDirectoryNames: directories);
            var conversationSummaryPlugin = kernel.ImportFunctions(new ConversationSummaryPlugin(kernel), nameof(ConversationSummaryPlugin));

            return kernel;
        });

        return services;
    }
}
