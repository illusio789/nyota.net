using Nyota.Domain;

namespace Nyota.Abstractions;

public interface IJournalHasher
{
    string ComputeHash(JournalEntry entry, string? prevHash);
}
