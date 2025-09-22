using System.Threading;
using System.Threading.Tasks;

using Nyota.Domain;

namespace Nyota.Abstractions;

public interface IExecutionProvider
{
    Task<ExecutionResult> SubmitAsync(Order order, CancellationToken ct);
}