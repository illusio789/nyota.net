using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;

using Nyota.Abstractions;
using Nyota.Domain;

namespace Nyota.Storage.File;

public sealed class FileUniverseRepository : IUniverseRepository
{
    private readonly string _folder;
    private readonly IFileSystem _fileSystem;

    public FileUniverseRepository(string folder, IFileSystem fileSystem)
    {
        _folder = folder;
        _fileSystem = fileSystem;
    }

    public async IAsyncEnumerable<Instrument> GetAllAsync(
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        if (!_fileSystem.Directory.Exists(_folder)) yield break;
        foreach (var file in _fileSystem.Directory.EnumerateFiles(_folder, "*.csv"))
        {
            foreach (var inst in ReadCsv(file)) yield return inst;
            await Task.Yield();
        }
    }

    public async Task<Instrument?> GetBySymbolAsync(Symbol symbol, CancellationToken ct)
    {
        await foreach (var i in GetAllAsync(ct))
            if (Symbol.Normalize(i.Symbol.Value).Equals(Symbol.Normalize(symbol.Value)))
                return i;
        return null;
    }

        private IEnumerable<Instrument> ReadCsv(string path)
    {
        using var s = _fileSystem.File.OpenRead(path);
        using var sr = new StreamReader(s);
        var header = sr.ReadLine();
        if (header is null) yield break;
        string? line;
        while ((line = sr.ReadLine()) is not null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var cols = line.Split(',');
            string sval(int i) => i < cols.Length ? cols[i] : string.Empty;

            decimal dval(int i) => decimal.TryParse(sval(i), NumberStyles.Any, CultureInfo.InvariantCulture, out var v)
                ? v
                : 0m;

            var symbol = new Symbol(sval(0));
            var assetClass = Enum.TryParse<AssetClass>(sval(1), true, out var ac) ? ac : AssetClass.commodity_etf;
            var venue = sval(2);
            var baseCcy = sval(3);
            var quoteCcy = sval(4);
            var tick = dval(5);
            var lot = dval(6);
            var isin = sval(8);
            var sedol = sval(9);
            var figi = sval(10);
            var leverage = dval(11);
            var notes = sval(13);
            yield return new Instrument(symbol, assetClass, InstrumentType.ETF, venue, baseCcy, quoteCcy, tick, lot,
                null, isin, sedol, figi, leverage, notes);
        }
    }
}
