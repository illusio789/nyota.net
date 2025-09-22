using Nyota.Core;

namespace Nyota.Abstractions;

public interface IRunConfigRepository
{
    Task<BacktestConfig> LoadAsync(string pathOrId, CancellationToken ct);
}