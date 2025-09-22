using System.Threading;
using System.Threading.Tasks;

using Nyota.Domain;

namespace Nyota.Abstractions;

public interface IJournalSink
{
    Task AppendAsync(JournalEntry entry, CancellationToken ct);
}