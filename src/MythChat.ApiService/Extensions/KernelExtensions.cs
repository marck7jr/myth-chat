using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

namespace MythChat.ApiService.Extensions;

public static class KernelExtensions
{
    private const string OrchestrationPluginName = "Orchestration";

    public static ISKFunction GetOrchestrationFunction(this IKernel kernel, string? functionName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(functionName);

        var function = kernel.Functions.GetFunction(OrchestrationPluginName, functionName);

        return function is null
            ? throw new ArgumentException($"Function '{functionName}' not found in plugin '{OrchestrationPluginName}'.")
            : function;
    }

    public static SKContext WithVariables<T>(this SKContext context, T? parameters)
    {
        if (parameters is null)
        {
            return context;
        }

        var variables = parameters.AsDictionary();

        foreach (var (key, value) in variables)
        {
            context.Variables[key] = value?.ToString() ?? string.Empty;
        }

        return context;
    }
}
