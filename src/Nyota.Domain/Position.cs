using System;

namespace Nyota.Domain;

public sealed record Position(
    Instrument Instrument,
    Quantity Qty,
    Price AvgPrice,
    DateTime OpenedAtUtc,
    DateTime UpdatedAtUtc,
    int HoldingPeriodDays);