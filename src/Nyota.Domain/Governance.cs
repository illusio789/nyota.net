using System.Collections.Generic;

namespace Nyota.Domain;

public sealed record Governance(
    int HoldingPeriodDays,
    IReadOnlyList<string> BlackoutTimesUtc,
    IReadOnlyList<Symbol> RestrictedList);