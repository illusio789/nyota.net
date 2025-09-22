namespace Nyota.Core;

public sealed record Governance(
    int HoldingPeriodDays,
    IReadOnlyList<string> BlackoutTimesUtc,
    IReadOnlyList<Symbol> RestrictedList);