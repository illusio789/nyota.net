namespace Nyota.Core;

public sealed record RuleResult(
    string RuleId,
    bool Passed,
    string Message,
    IReadOnlyDictionary<string, object>? Data = null);