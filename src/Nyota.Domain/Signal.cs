namespace Nyota.Domain;

public sealed record Signal(
    string StrategyId,
    Instrument Instrument,
    DateTime TimestampUtc,
    OrderSide Direction,
    double Strength,
    string ReasonCode,
    IReadOnlyList<string> Tags,
    IReadOnlyDictionary<string, object> Debug);