namespace Nyota.Core;

public sealed record Instrument(
    Symbol Symbol,
    AssetClass AssetClass,
    InstrumentType Type,
    string Venue,
    string BaseCcy,
    string? QuoteCcy,
    decimal TickSize,
    decimal LotSize,
    Money? MinNotional,
    string? ISIN,
    string? SEDOL,
    string? FIGI,
    decimal Leverage,
    string? Notes);