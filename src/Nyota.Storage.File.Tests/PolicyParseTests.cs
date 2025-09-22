using System.IO;
using System.Threading.Tasks;

using Xunit;

namespace Nyota.Storage.File;

public class PolicyParseTests
{
    [Fact]
    public async Task LoadsPolicyFromYaml()
    {
        var path = Path.GetTempFileName();
        var yaml = """
                   policy:
                     allowed_classes: [commodity_etf, currency_etf]
                     venues_whitelist: [NYSE]
                   risk:
                     max_position_notional_pct: 0.02
                     max_asset_class_notional_pct: 0.40
                     max_daily_turnover_pct: 0.10
                     min_avg_daily_volume_usd: 2000000
                     max_spread_bps: 25
                     leverage_allowed: false
                     shorting_allowed: false
                   governance:
                     holding_period_days: 1
                     blackout_times_utc: ["21:55-22:10"]
                     restricted_list: [GLD]
                   execution:
                     default_time_in_force: DAY
                     eod_batch_for_etfs: true
                   version: 1.0.0
                   """;
        await System.IO.File.WriteAllTextAsync(path, yaml);
        var repo = new FilePolicyRepository(path);
        var p = await repo.GetAsync(default);
        Assert.Equal("1.0.0", p.Version);
        System.IO.File.Delete(path);
    }
}
