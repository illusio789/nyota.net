using Nyota.Abstractions;

using System.Linq;

using Nyota.Domain;

namespace Nyota.Core
{
    public sealed class PaperTrader
    {
        private readonly IClock _clock;
        private readonly IDataProvider _data;
        private readonly IExecutionProvider _exec;
        private readonly IPortfolioRepository _pf;

        public PaperTrader(IClock c, IDataProvider d, IExecutionProvider e, IPortfolioRepository p)
        {
            _clock = c;
            _data = d;
            _exec = e;
            _pf = p;
        }

        public async System.Threading.Tasks.Task RunAsync(string pfId, Instrument inst, IStrategy strat,
            System.Threading.CancellationToken ct)
        {
            var p = await _pf.GetAsync(pfId, ct);
            await foreach (var _ in _clock.NowTicksAsync(ct))
            {
                var bars = await _data
                    .GetBarsAsync(
                        new BarRequest(inst, System.DateTime.UtcNow.AddDays(-200), System.DateTime.UtcNow, Resolution.D,
                            false), ct).ToListAsync(ct);
                if (bars.Count == 0) continue;
                var sigs = await strat.GenerateAsync(inst, bars, ct).ToListAsync(ct);
                var last = sigs.LastOrDefault();
                if (last is null) continue;
                var px = bars[^1].Close;
                var cash = p.Equity.Amount;
                var qty = System.Math.Round((cash * 0.1m) / px, 4);
                var o = new Order(System.Guid.NewGuid().ToString("N"), inst, last.Direction, OrderType.Market,
                    new Quantity(qty, inst.LotSize), null, TimeInForce.DAY, System.DateTime.UtcNow, strat.Id);
                var r = await _exec.SubmitAsync(o, ct);
                var notional = r.Fills.Sum(f => f.Price.Value * f.Quantity.Value);
                var newCash = last.Direction == OrderSide.Buy ? cash - notional : cash + notional;
                p = p with { Equity = new Money(newCash, p.Equity.Currency) };
                await _pf.SaveAsync(pfId, p, ct);
            }
        }
    }
}
