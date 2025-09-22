using System.Threading;
using System.Threading.Tasks;

using Nyota.Domain;

namespace Nyota.Abstractions;

public interface IRunConfigRepository
{
    Task<BacktestConfig> LoadAsync(string pathOrId, CancellationToken ct);
}
