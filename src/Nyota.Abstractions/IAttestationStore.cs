using Nyota.Core;

namespace Nyota.Abstractions;

public interface IAttestationStore
{
    Task SaveDailyAsync(Attestation attestation, CancellationToken ct);
    Task<Attestation?> GetByDateAsync(DateTime dateUtc, CancellationToken ct);
}