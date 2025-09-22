using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Nyota.Domain;

namespace Nyota.Abstractions;

public interface IPortfolioRepository
{
    Task<Portfolio> GetAsync(string portfolioId, CancellationToken ct);
    Task SaveAsync(string portfolioId, Portfolio portfolio, CancellationToken ct);
}

public interface IDataProvider
{
    IAsyncEnumerable<MarketDataBar> GetBarsAsync(BarRequest req, CancellationToken ct);
}

public interface IStrategy
{
    string Id { get; }

    IAsyncEnumerable<Signal> GenerateAsync(Instrument instrument,
        IEnumerable<MarketDataBar> history, CancellationToken ct);
}

public interface IExecutionProvider
{
    Task<ExecutionResult> SubmitAsync(Order order, CancellationToken ct);
}

public interface IClock
{
    IAsyncEnumerable<DateTime> NowTicksAsync(CancellationToken ct);
}
