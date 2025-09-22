using System.IO.Abstractions;
using System.Text.Json;

using Nyota.Abstractions;
using Nyota.Domain;

namespace Nyota.Storage.File;

public sealed class FileJournal : IJournalSink, IJournalSource
{
    private readonly string _path;
    private readonly IFileSystem _fileSystem;
    private readonly IJournalHasher _hasher;

    public FileJournal(string path, IFileSystem fileSystem, IJournalHasher? hasher = null)
    {
        _path = path;
        _fileSystem = fileSystem;
        _hasher = hasher ?? new SimpleJournalHasher();
    }

    public async Task AppendAsync(JournalEntry e, CancellationToken ct)
    {
        var tip = await GetTipAsync(ct);
        var hash = _hasher.ComputeHash(e, tip?.Hash);
        var chained = e with { PrevHash = tip?.Hash, Hash = hash };
        var json = JsonSerializer.Serialize(chained);
        await _fileSystem.File.AppendAllTextAsync(_path, json + Environment.NewLine, ct);
    }

    public async IAsyncEnumerable<JournalEntry> GetRecentAsync(int limit,
        [System.Runtime.CompilerServices.EnumeratorCancellation]
        CancellationToken ct)
    {
        if (!_fileSystem.File.Exists(_path)) yield break;
        var lines = (await _fileSystem.File.ReadAllLinesAsync(_path, ct)).Reverse().Take(limit).Reverse();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var e = JsonSerializer.Deserialize<JournalEntry>(line);
            if (e != null) yield return e;
        }
    }

    public async Task<JournalEntry?> GetTipAsync(CancellationToken ct)
    {
        if (!_fileSystem.File.Exists(_path)) return null;
        var lines = await _fileSystem.File.ReadAllLinesAsync(_path, ct);
        for (int i = lines.Length - 1; i >= 0; i--)
        {
            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;
            var e = JsonSerializer.Deserialize<JournalEntry>(line);
            if (e != null) return e;
        }

        return null;
    }
}
