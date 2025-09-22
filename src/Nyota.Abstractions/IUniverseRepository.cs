using Nyota.Core;

namespace Nyota.Abstractions;

public interface IUniverseRepository
{
    IAsyncEnumerable<Instrument> GetAllAsync(CancellationToken ct);
    Task<Instrument?> GetBySymbolAsync(Symbol symbol, CancellationToken ct);
}