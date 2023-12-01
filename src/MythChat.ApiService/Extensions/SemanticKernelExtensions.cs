using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

using MythChat.ApiService.Configuration;

namespace MythChat.ApiService.Extensions;

public static class SemanticKernelExtensions
{
    public static IServiceCollection AddSemanticKernel(this IServiceCollection services)
    {
        services.AddSingleton(provider =>
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

            var plugins = kernel.ImportSemanticFunctionsFromDirectory(path, pluginDirectoryNames: directories);

            return kernel;
        });

        return services;
    }
}
