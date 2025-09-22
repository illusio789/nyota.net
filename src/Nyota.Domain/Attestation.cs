using System;
using System.Collections.Generic;

namespace Nyota.Domain;

public sealed record Attestation(
    DateTime DateUtc,
    string PolicyVersion,
    IReadOnlyList<Position> Positions,
    IReadOnlyList<RuleResult> Breaches,
    string HashChainTip);
