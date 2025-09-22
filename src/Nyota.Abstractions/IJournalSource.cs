using Nyota.Core;

namespace Nyota.Abstractions;

public interface IJournalSource
{
    IAsyncEnumerable<JournalEntry> GetRecentAsync(int limit, CancellationToken ct);
    Task<JournalEntry?> GetTipAsync(CancellationToken ct);
}