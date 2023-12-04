using System.Text.Json;

using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

using MythChat.Web.Contracts;
using MythChat.Web.Models;

namespace MythChat.Web.Services;

public class HistoryService(
    ILogger<HistoryService> logger,
    ProtectedLocalStorage protectedLocalStorage) : IHistoryService
{
    private const string Key = "history";

    public async Task<IEnumerable<HistoryEntryModel>> AddEntryAsync(HistoryEntryModel? historyEntryModel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(historyEntryModel);

        try
        {
            var entries = await GetEntriesAsync(cancellationToken);

            if (entries.Contains(historyEntryModel))
            {
                return entries;
            }

            var newEntries = new List<HistoryEntryModel>(entries)
            {
                historyEntryModel
            };

            var json = JsonSerializer.Serialize(newEntries);
            await protectedLocalStorage.SetAsync(Key, json);

            return newEntries;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add history entry");
        }

        return [];
    }

    public async Task<HistoryEntryModel?> GetEntryAsync(string? channel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(channel);

        try
        {
            var entries = await GetEntriesAsync(cancellationToken);
            var entry = entries.FirstOrDefault(x => x.Channel == channel);

            return entry;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get history entry");
        }

        return null;
    }

    public async Task<IEnumerable<HistoryEntryModel>> GetEntriesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await protectedLocalStorage.GetAsync<string?>(Key);
            var history = result.Value;

            if (string.IsNullOrEmpty(history))
            {
                return [];
            }

            var entries = JsonSerializer.Deserialize<IEnumerable<HistoryEntryModel>>(history) ?? [];
            return entries;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get history entries");
        }

        return [];
    }
}
