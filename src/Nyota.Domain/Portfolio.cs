namespace Nyota.Domain;

public sealed record Portfolio(
    Money Equity,
    IReadOnlyDictionary<string, Money> CashByCcy,
    IReadOnlyDictionary<Symbol, Position> Positions,
    Money RealizedPnl,
    Money UnrealizedPnl);