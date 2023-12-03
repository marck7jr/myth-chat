namespace MythChat.ApiService.Configuration;

public class ChatOptions
{
    public const string Name = "Chat";

    public IEnumerable<Agent> Agents { get; set; } = [];

    public class Agent
    {
        public string Description { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Dictionary<string, object?> Metadata { get; set; } = [];
        public string Type { get; set; } = string.Empty;
    }
}
