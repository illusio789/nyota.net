using Nyota.Core;

namespace Nyota.Abstractions;

public interface ITradeLedger
{
    Task AppendTradeAsync(Trade trade, CancellationToken ct);
    IAsyncEnumerable<Trade> GetRecentAsync(int limit, CancellationToken ct);
}