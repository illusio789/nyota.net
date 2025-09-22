using Nyota.Core;

namespace Nyota.Abstractions;

public interface IJournalSink
{
    Task AppendAsync(JournalEntry entry, CancellationToken ct);
}