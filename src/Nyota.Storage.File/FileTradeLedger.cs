using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Nyota.Abstractions;
using Nyota.Domain;

namespace Nyota.Storage.File;

public sealed class FileTradeLedger : ITradeLedger
{
    private readonly string _path;
    private readonly IFileSystem _fileSystem;

    public FileTradeLedger(string path, IFileSystem fileSystem)
    {
        _path = path;
        _fileSystem = fileSystem;
    }

    public async Task AppendTradeAsync(Trade t, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(t);
        await _fileSystem.File.AppendAllTextAsync(_path, json + Environment.NewLine, ct);
    }

    public async IAsyncEnumerable<Trade> GetRecentAsync(int limit,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        if (!_fileSystem.File.Exists(_path)) yield break;
        var lines = (await _fileSystem.File.ReadAllLinesAsync(_path, ct)).Reverse().Take(limit).Reverse();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var t = JsonSerializer.Deserialize<Trade>(line);
            if (t != null) yield return t;
        }
    }
}
