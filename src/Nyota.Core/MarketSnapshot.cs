namespace Nyota.Core;

public sealed record MarketSnapshot(
    Instrument Instrument,
    Price? Bid,
    Price? Ask,
    Price? Last,
    int SpreadBps,
    decimal RollingAdvUsd);