using System;
using System.Collections.Generic;

using Nyota.Core;
using Nyota.Storage.File;

using Xunit;

namespace Nyota.Storage.File.Tests;

public class JournalTests
{
    [Fact]
    public async System.Threading.Tasks.Task HashChain_LinksEntries()
    {
        var path = System.IO.Path.GetTempFileName();
        try
        {
            var j = new FileJournal(path);
            var e1 = new JournalEntry(DateTime.UtcNow, "Info", "Test", new Dictionary<string, string> { ["a"] = "1" }, "{}", "", null);
            await j.AppendAsync(e1, default);
            var e2 = new JournalEntry(DateTime.UtcNow.AddSeconds(1), "Info", "Test", new Dictionary<string, string> { ["b"] = "2" }, "{}", "", null);
            await j.AppendAsync(e2, default);
            var tip = await j.GetTipAsync(default);
            Assert.NotNull(tip);
            Assert.NotNull(tip!.PrevHash);
            Assert.NotEqual(tip.Hash, tip.PrevHash);
        }
        finally { try { System.IO.File.Delete(path); } catch {} }
    }
}
