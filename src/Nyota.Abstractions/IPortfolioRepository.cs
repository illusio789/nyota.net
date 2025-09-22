using Nyota.Core;

namespace Nyota.Abstractions;

public interface IPortfolioRepository
{
    Task<Portfolio> GetAsync(string portfolioId, CancellationToken ct);
    Task SaveAsync(string portfolioId, Portfolio portfolio, CancellationToken ct);
}