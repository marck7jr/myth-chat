using System.Text.Json;

namespace MythChat.ApiService.Extensions;

public static class ObjectExtensions
{
    internal static Dictionary<string, object?> AsDictionary(this object? obj)
    {
        if (obj is null)
        {
            return [];
        }

        var json = JsonSerializer.Serialize(obj);
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, object?>>(json);

        return dictionary ?? [];
    }
}
