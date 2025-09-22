using System;
using System.Collections.Generic;
using System.Threading;

namespace Nyota.Abstractions;

public interface IClock
{
    IAsyncEnumerable<DateTime> NowTicksAsync(CancellationToken ct);
}