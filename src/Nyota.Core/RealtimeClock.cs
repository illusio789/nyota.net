using Nyota.Abstractions;

namespace Nyota.Core
{
    public sealed class RealtimeClock : IClock
    {
        private readonly System.TimeSpan _interval;
        public RealtimeClock(System.TimeSpan interval) => _interval = interval;

        public async System.Collections.Generic.IAsyncEnumerable<System.DateTime> NowTicksAsync(
            System.Threading.CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                yield return System.DateTime.UtcNow;
                await System.Threading.Tasks.Task.Delay(_interval, ct);
            }
        }
    }
}
