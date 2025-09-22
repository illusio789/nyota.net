using System.Threading;
using System.Threading.Tasks;

using Nyota.Domain;

namespace Nyota.Abstractions;

public interface IPortfolioRepository
{
    Task<Portfolio> GetAsync(string portfolioId, CancellationToken ct);
    Task SaveAsync(string portfolioId, Portfolio portfolio, CancellationToken ct);
}
