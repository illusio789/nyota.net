namespace Nyota.Domain;

public sealed record RunSummary(
    string RunId,
    DateTime StartedUtc,
    DateTime EndedUtc,
    Policy Policy,
    IReadOnlyList<string> Strategies,
    SimulationReport? BacktestReport);
