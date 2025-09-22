namespace Nyota.Domain;

public sealed record Trade(string TradeId, Order Order, ExecutionResult Execution);