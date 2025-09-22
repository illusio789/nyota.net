using System;
using System.Threading;
using System.Threading.Tasks;

using Nyota.Domain;

namespace Nyota.Abstractions;

public interface IAttestationStore
{
    Task SaveDailyAsync(Attestation attestation, CancellationToken ct);
    Task<Attestation?> GetByDateAsync(DateTime dateUtc, CancellationToken ct);
}
