using System.Threading;
using System.Threading.Tasks;

using Nyota.Domain;

namespace Nyota.Abstractions
{
    public interface IPolicyRepository
    {
        Task<Policy> GetAsync(CancellationToken ct);
        Task SaveAsync(Policy policy, CancellationToken ct);
    }
}
