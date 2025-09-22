using System.Text;
using System.Text.Json;

using Nyota.Abstractions;
using Nyota.Domain;

namespace Nyota.Storage.File;

public sealed class SimpleJournalHasher : IJournalHasher
{
    public string ComputeHash(JournalEntry e, string? prev)
    {
        var s = $"{prev}|{e.TimestampUtc:O}|{e.Level}|{e.Category}|{JsonSerializer.Serialize(e.EntityRefs)}|{e.PayloadJson}";
        using var sha = System.Security.Cryptography.SHA256.Create();
        return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(s)));
    }
}