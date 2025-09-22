namespace Nyota.Core;

public sealed record SimulationReport(
    string RunId,
    SimulationKpi Kpi,
    IReadOnlyList<(DateTime Utc, decimal Equity)> EquityCurve,
    IReadOnlyDictionary<string, object> PerStrategyStats,
    IReadOnlyDictionary<string, object> PerRuleStats);