using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Threading;
using System.Threading.Tasks;

using Nyota.Domain;

using Xunit;

namespace Nyota.Storage.File;

public class UniverseTests
{
    [Fact]
    public async Task Reads_SingleRowCsv()
    {
        var fileSystem = new MockFileSystem();
        var folder = fileSystem.Path.Combine(fileSystem.Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        fileSystem.Directory.CreateDirectory(folder);

        const string csv = "symbol,asset_class,venue,base_ccy,quote_ccy,tick_size,lot_size,min_notional,isin,sedol,figi,leverage,source,notes\n" +
                           "GLD,commodity_etf,NYSE,USD,,0.01,1,0,US78463V1070,B05M4X2,BBG000BDTBL9,0,manual,SPDR Gold Shares\n";

        await fileSystem.File.WriteAllTextAsync(fileSystem.Path.Combine(folder, "universe.csv"), csv);
        var repo = new FileUniverseRepository(folder, fileSystem);
        var inst = await repo.GetBySymbolAsync(new Symbol("GLD"), CancellationToken.None);
        Assert.NotNull(inst);
        Assert.Equal("NYSE", inst!.Venue);
    }
}
