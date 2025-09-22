using System.Text.Json;

using Nyota.Abstractions;
using Nyota.Core;

namespace Nyota.Storage.File;

public sealed class FileTradeLedger : ITradeLedger
{
    private readonly string _path;
    public FileTradeLedger(string path){ _path = path; }
    public async Task AppendTradeAsync(Trade t, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(t);
        await System.IO.File.AppendAllTextAsync(_path, json + Environment.NewLine, ct);
    }
    public async IAsyncEnumerable<Trade> GetRecentAsync(int limit, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        if (!System.IO.File.Exists(_path)) yield break;
        var lines = (await System.IO.File.ReadAllLinesAsync(_path, ct)).Reverse().Take(limit).Reverse();
        foreach (var line in lines) { if (string.IsNullOrWhiteSpace(line)) continue; var t = JsonSerializer.Deserialize<Trade>(line); if (t != null) yield return t; }
    }
}