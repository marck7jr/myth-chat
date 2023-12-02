using System.Text.Json;

namespace MythChat.ApiService.Configuration;

public class SemanticKernelOptions
{
    internal const string Name = "SemanticKernel";

    public IEnumerable<Agent> Agents { get; set; } = [];
    public string SecretKey { get; set; } = string.Empty;
    public string ModelId { get; set; } = string.Empty;

    public class Agent
    {
        public string Description { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public JsonElement Metadata { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
