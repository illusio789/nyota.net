using System.Globalization;

using Nyota.Abstractions;
using Nyota.Domain;

namespace Nyota.Storage.File;

public sealed class FileUniverseRepository : IUniverseRepository
{
    private readonly string _folder;
    public FileUniverseRepository(string folder){ _folder = folder; }
    public async IAsyncEnumerable<Instrument> GetAllAsync([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        if (!Directory.Exists(_folder)) yield break;
        foreach (var file in Directory.EnumerateFiles(_folder, "*.csv"))
        {
            foreach (var inst in ReadCsv(file)) yield return inst;
            await Task.Yield();
        }
    }
    public async Task<Instrument?> GetBySymbolAsync(Symbol symbol, CancellationToken ct)
    {
        await foreach (var i in GetAllAsync(ct)) if (Symbol.Normalize(i.Symbol.Value).Equals(Symbol.Normalize(symbol.Value))) return i;
        return null;
    }
    private static IEnumerable<Instrument> ReadCsv(string path)
    {
        using var sr = new StreamReader(path);
        var header = sr.ReadLine();
        if (header is null) yield break;
        string? line;
        while ((line = sr.ReadLine()) is not null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var cols = line.Split(',');
            string sval(int i)=> i<cols.Length? cols[i]: string.Empty;
            decimal dval(int i)=> decimal.TryParse(sval(i), NumberStyles.Any, CultureInfo.InvariantCulture, out var v)? v: 0m;
            var symbol = new Symbol(sval(0));
            var assetClass = Enum.TryParse<AssetClass>(sval(1), true, out var ac)? ac: AssetClass.commodity_etf;
            var venue = sval(2); var baseCcy = sval(3); var quoteCcy = sval(4);
            var tick = dval(5); var lot = dval(6);
            var isin = sval(8); var sedol = sval(9); var figi = sval(10);
            var leverage = dval(11); var notes = sval(13);
            yield return new Instrument(symbol, assetClass, InstrumentType.ETF, venue, baseCcy, quoteCcy, tick, lot, null, isin, sedol, figi, leverage, notes);
        }
    }
}
