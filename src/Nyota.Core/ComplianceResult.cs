namespace Nyota.Core;

public sealed record ComplianceResult(
    ComplianceDecision Decision,
    IReadOnlyList<RuleResult> RuleResults,
    string ReceiptHash);