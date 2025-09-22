using System.Collections.Generic;

namespace Nyota.Domain;

public sealed record OrderSize(
    Instrument Instrument,
    Quantity Quantity,
    Money Notional,
    IReadOnlyDictionary<string, object>? Constraints = null);