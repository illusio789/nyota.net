using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Nyota.Domain;

namespace Nyota.Abstractions;

public interface IUniverseRepository
{
    IAsyncEnumerable<Instrument> GetAllAsync(CancellationToken ct);
    Task<Instrument?> GetBySymbolAsync(Symbol symbol, CancellationToken ct);
}
