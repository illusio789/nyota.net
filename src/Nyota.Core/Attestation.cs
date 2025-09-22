namespace Nyota.Core;

public sealed record Attestation(
    DateTime DateUtc,
    string PolicyVersion,
    IReadOnlyList<Position> Positions,
    IReadOnlyList<RuleResult> Breaches,
    string HashChainTip);