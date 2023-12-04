using MythChat.Web.Models;

namespace MythChat.Web.Contracts;

public interface IHistoryService
{
    public Task<IEnumerable<HistoryEntryModel>> AddEntryAsync(HistoryEntryModel historyEntryModel, CancellationToken cancellationToken = default);
    public Task<IEnumerable<HistoryEntryModel>> GetEntriesAsync(CancellationToken cancellationToken = default);
    Task<HistoryEntryModel?> GetEntryAsync(string? channel, CancellationToken cancellationToken = default);
}
