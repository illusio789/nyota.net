using System.Collections.Generic;
using System.Threading;

using Nyota.Domain;

namespace Nyota.Abstractions;

public interface IDataProvider
{
    IAsyncEnumerable<MarketDataBar> GetBarsAsync(BarRequest req, CancellationToken ct);
}