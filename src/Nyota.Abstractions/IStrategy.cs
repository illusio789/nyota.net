using System.Collections.Generic;
using System.Threading;

using Nyota.Domain;

namespace Nyota.Abstractions;

public interface IStrategy
{
    string Id { get; }

    IAsyncEnumerable<Signal> GenerateAsync(Instrument instrument,
        IEnumerable<MarketDataBar> history, CancellationToken ct);
}