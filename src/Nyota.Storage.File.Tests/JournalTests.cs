using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Threading;

using Nyota.Domain;

using Xunit;

namespace Nyota.Storage.File;

public class JournalTests
{
    [Fact]
    public async System.Threading.Tasks.Task HashChain_LinksEntries()
    {
        var fileSystem = new MockFileSystem();

        var j = new FileJournal("journal", fileSystem);

        var e1 = new JournalEntry(
            DateTime.UtcNow,
            "Info",
            "Test",
            new Dictionary<string, string> { ["a"] = "1" },
            "{}",
            "",
            null);

        await j.AppendAsync(e1, CancellationToken.None);

        var e2 = new JournalEntry(
            DateTime.UtcNow.AddSeconds(1),
            "Info",
            "Test",
            new Dictionary<string, string> { ["b"] = "2" },
            "{}",
            "",
            null);

        await j.AppendAsync(e2, CancellationToken.None);

        var tip = await j.GetTipAsync(CancellationToken.None);

        Assert.NotNull(tip);
        Assert.NotNull(tip!.PrevHash);
        Assert.NotEqual(tip.Hash, tip.PrevHash);
    }
}
