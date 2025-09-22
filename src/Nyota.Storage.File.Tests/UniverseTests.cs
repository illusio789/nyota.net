using System;
using System.IO;
using System.Threading.Tasks;

using Nyota.Core;
using Nyota.Storage.File;

using Xunit;

namespace Nyota.Storage.File.Tests;

public class UniverseTests
{
    [Fact]
    public async Task Reads_SingleRowCsv()
    {
        var folder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(folder);
        try
        {
            var csv = "symbol,asset_class,venue,base_ccy,quote_ccy,tick_size,lot_size,min_notional,isin,sedol,figi,leverage,source,notes\n" +
                      "GLD,commodity_etf,NYSE,USD,,0.01,1,0,US78463V1070,B05M4X2,BBG000BDTBL9,0,manual,SPDR Gold Shares\n";
            await System.IO.File.WriteAllTextAsync(Path.Combine(folder, "universe.csv"), csv);
            var repo = new FileUniverseRepository(folder);
            var inst = await repo.GetBySymbolAsync(new Symbol("GLD"), default);
            Assert.NotNull(inst);
            Assert.Equal("NYSE", inst!.Venue);
        }
        finally { try { Directory.Delete(folder, true); } catch {} }
    }
}
