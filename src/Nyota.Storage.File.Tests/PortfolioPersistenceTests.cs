using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;

using Nyota.Domain;

using Xunit;

namespace Nyota.Storage.File;

public class PortfolioPersistenceTests
{
    [Fact]
    public async Task SaveAndLoad_Works()
    {
        var fileSystem = new MockFileSystem();
        var folder = fileSystem.Path.Combine(System.IO.Path.GetTempPath(), System.Guid.NewGuid().ToString("N"));
        fileSystem.AddDirectory(folder);

        var repo = new FilePortfolioRepository(folder, fileSystem);
        var p = new Portfolio(new Money(123456m, "USD"), new Dictionary<string, Money>(),
            new Dictionary<Symbol, Position>(), new Money(0, "USD"), new Money(0, "USD"));
        await repo.SaveAsync("default", p, default);
        var loaded = await repo.GetAsync("default", default);
        Assert.Equal(123456m, loaded.Equity.Amount);
    }
}
