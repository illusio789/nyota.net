namespace Nyota.Core;

public sealed record Position(
    Instrument Instrument,
    Quantity Qty,
    Price AvgPrice,
    DateTime OpenedAtUtc,
    DateTime UpdatedAtUtc,
    int HoldingPeriodDays);