namespace Nyota.Core;

public sealed record MarketDataBar(
    DateTime Utc,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    decimal Volume,
    Resolution Resolution);