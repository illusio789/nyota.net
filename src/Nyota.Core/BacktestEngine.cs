using Nyota.Abstractions;
using Nyota.Domain;

namespace Nyota.Core;

public sealed class BacktestEngine
{
    private readonly IDataProvider _data;
    private readonly IPortfolioRepository _pf;

    public BacktestEngine(IDataProvider d, IPortfolioRepository p)
    {
        _data = d;
        _pf = p;
    }

    public async Task<(decimal startEq, decimal endEq)> RunAsync(string pfId,
        Instrument inst, IStrategy strat, DateTime start, DateTime end,
        CancellationToken ct)
    {
        var bars = await _data.GetBarsAsync(new BarRequest(inst, start, end, Resolution.D, false), ct)
            .ToListAsync(ct);
        var sigs = await strat.GenerateAsync(inst, bars, ct).ToListAsync(ct);
        var p = await _pf.GetAsync(pfId, ct);
        decimal cash = p.Equity.Amount;
        decimal qty = 0;
        foreach (var b in bars)
        {
            foreach (var s in sigs.Where(s => s.TimestampUtc.Date == b.Utc.Date))
            {
                if (s.Direction == OrderSide.Buy && cash > 0)
                {
                    var notional = Math.Min(cash * 0.1m, cash);
                    var q = Math.Round(notional / b.Close, 4);
                    qty += q;
                    cash -= notional;
                }
                else if (s.Direction == OrderSide.Sell && qty > 0)
                {
                    cash += qty * b.Close;
                    qty = 0;
                }
            }
        }

        var endEq = cash + qty * bars.Last().Close;
        return (p.Equity.Amount, endEq);
    }
}
