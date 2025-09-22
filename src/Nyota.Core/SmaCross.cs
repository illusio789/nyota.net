using Nyota.Abstractions;

using System.Linq;

using Nyota.Domain;

namespace Nyota.Core
{
    public sealed class SmaCross : IStrategy
    {
        public string Id => "sma_cross_20_100";

        public async System.Collections.Generic.IAsyncEnumerable<Signal> GenerateAsync(Instrument instrument,
            System.Collections.Generic.IEnumerable<MarketDataBar> history, System.Threading.CancellationToken ct)
        {
            var bars = history.OrderBy(b => b.Utc).ToArray();
            if (bars.Length < 100) yield break;
            decimal SMA(int i, int n) => bars.Skip(i - n + 1).Take(n).Average(b => b.Close);
            for (int i = 100; i < bars.Length; i++)
            {
                var fast = SMA(i, 20);
                var slow = SMA(i, 100);
                var pf = SMA(i - 1, 20);
                var ps = SMA(i - 1, 100);
                if (pf <= ps && fast > slow)
                    yield return new Signal(Id, instrument, bars[i].Utc, OrderSide.Buy, 1, "CROSS_UP",
                        System.Array.Empty<string>(), new System.Collections.Generic.Dictionary<string, object>());
                else if (pf >= ps && fast < slow)
                    yield return new Signal(Id, instrument, bars[i].Utc, OrderSide.Sell, 1, "CROSS_DOWN",
                        System.Array.Empty<string>(), new System.Collections.Generic.Dictionary<string, object>());
                await System.Threading.Tasks.Task.Yield();
            }
        }
    }
}
