namespace MythChat.ApiService.Configuration;

public class SemanticKernelOptions
{
    internal const string Name = "SemanticKernel";

    public string SecretKey { get; set; } = string.Empty;
    public string ModelId { get; set; } = string.Empty;
}
