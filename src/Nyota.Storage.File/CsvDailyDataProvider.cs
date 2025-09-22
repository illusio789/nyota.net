using System.Globalization;
using System.IO;
using System.IO.Abstractions;

using Nyota.Abstractions;
using Nyota.Domain;

namespace Nyota.Storage.File
{
    public sealed class CsvDailyDataProvider : IDataProvider
    {
        private readonly string _folder;
        private readonly IFileSystem _fileSystem;

        public CsvDailyDataProvider(string folder, IFileSystem fileSystem)
        {
            _folder = folder;
            _fileSystem = fileSystem;
        }

        public async System.Collections.Generic.IAsyncEnumerable<MarketDataBar> GetBarsAsync(BarRequest req,
            [System.Runtime.CompilerServices.EnumeratorCancellation]
            System.Threading.CancellationToken ct)
        {
            var path = _fileSystem.Path.Combine(_folder, $"{req.Instrument.Symbol.Value}.csv");
            if (!_fileSystem.File.Exists(path)) yield break;

            using var s = _fileSystem.File.OpenRead(path);
            using var sr = new StreamReader(s);
            var header = await sr.ReadLineAsync();
            string? line;
            while ((line = await sr.ReadLineAsync()) is not null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var c = line.Split(',');
                var dt = System.DateTime.SpecifyKind(System.DateTime.Parse(c[0], CultureInfo.InvariantCulture),
                    System.DateTimeKind.Utc);
                if (dt < req.StartUtc || dt > req.EndUtc) continue;
                yield return new MarketDataBar(dt, decimal.Parse(c[1], CultureInfo.InvariantCulture),
                    decimal.Parse(c[2], CultureInfo.InvariantCulture),
                    decimal.Parse(c[3], CultureInfo.InvariantCulture),
                    decimal.Parse(c[5], CultureInfo.InvariantCulture),
                    decimal.Parse(c[6], CultureInfo.InvariantCulture), req.Resolution);
                await System.Threading.Tasks.Task.Yield();
            }
        }
    }
}
