using Nyota.Core;

namespace Nyota.Abstractions
{
    public interface IPolicyRepository
    {
        Task<Policy> GetAsync(CancellationToken ct);
        Task SaveAsync(Policy policy, CancellationToken ct);
    }
}
