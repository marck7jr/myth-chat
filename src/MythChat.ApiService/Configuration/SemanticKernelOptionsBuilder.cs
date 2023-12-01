using Microsoft.Extensions.Options;

namespace MythChat.ApiService.Configuration;

public class SemanticKernelOptionsBuilder(IConfiguration configuration) : IConfigureOptions<SemanticKernelOptions>
{
    public void Configure(SemanticKernelOptions options)
    {
        configuration.Bind(SemanticKernelOptions.Name, options);
    }
}
