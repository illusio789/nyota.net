using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Nyota.Domain;

namespace Nyota.Abstractions;

public interface ITradeLedger
{
    Task AppendTradeAsync(Trade trade, CancellationToken ct);
    IAsyncEnumerable<Trade> GetRecentAsync(int limit, CancellationToken ct);
}
