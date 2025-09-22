using System.Collections.Generic;

namespace Nyota.Domain;

public sealed record ComplianceResult(
    ComplianceDecision Decision,
    IReadOnlyList<RuleResult> RuleResults,
    string ReceiptHash);
