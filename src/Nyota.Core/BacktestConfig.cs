namespace Nyota.Core;

public sealed record BacktestConfig(
    ClockConfig Clock,
    IReadOnlyList<AssetClass> AssetClasses,
    IReadOnlyList<StrategyConfig> Strategies,
    DataConfig Data,
    ExecutionConfig Execution,
    decimal InitialEquity,
    IReadOnlyDictionary<string, object>? TransactionCosts = null);