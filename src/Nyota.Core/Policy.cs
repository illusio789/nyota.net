namespace Nyota.Core;

public sealed record Policy(
    IReadOnlyList<AssetClass> AllowedClasses,
    IReadOnlyList<string> VenuesWhitelist,
    RiskLimits Risk,
    Governance Governance,
    ExecutionRules Execution,
    string Version = "1.0.0");