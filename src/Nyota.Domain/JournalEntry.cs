namespace Nyota.Domain;

public sealed record JournalEntry(
    DateTime TimestampUtc,
    string Level,
    string Category,
    IReadOnlyDictionary<string, string> EntityRefs,
    string PayloadJson,
    string Hash,
    string? PrevHash);