using System;
using System.Collections.Generic;

namespace Nyota.Domain;

public sealed record Order(
    string Id,
    Instrument Instrument,
    OrderSide Side,
    OrderType Type,
    Quantity Qty,
    Price? LimitPrice,
    TimeInForce Tif,
    DateTime SubmittedAtUtc,
    string StrategyId,
    IReadOnlyList<string>? ClientTags = null);