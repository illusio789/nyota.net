using Nyota.Abstractions;
using Nyota.Domain;

namespace Nyota.Core
{
    public sealed class PaperExecutionProvider : IExecutionProvider
    {
        private readonly IDataProvider _data;
        public PaperExecutionProvider(IDataProvider d) => _data = d;

        public async Task<ExecutionResult> SubmitAsync(Order o,
            CancellationToken ct)
        {
            var list = await _data
                .GetBarsAsync(
                    new BarRequest(o.Instrument, DateTime.UtcNow.AddDays(-3), DateTime.UtcNow,
                        Resolution.D, false), ct).ToListAsync(ct);
            var px = list.Count > 0 ? list[^1].Close : o.LimitPrice?.Value ?? 0m;
            var fill = new Fill(DateTime.UtcNow, new Price(px, o.Instrument.BaseCcy), o.Qty);
            return new ExecutionResult(o.Id, ExecutionStatus.Filled, new[] { fill }, new Money(0, o.Instrument.BaseCcy),
                0, "Paper", null);
        }
    }
}
