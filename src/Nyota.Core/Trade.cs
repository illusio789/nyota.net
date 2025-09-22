namespace Nyota.Core;

public sealed record Trade(string TradeId, Order Order, ExecutionResult Execution);