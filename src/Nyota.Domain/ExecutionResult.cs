using System.Collections.Generic;

namespace Nyota.Domain;

public sealed record ExecutionResult(
    string OrderId,
    ExecutionStatus Status,
    IReadOnlyList<Fill> Fills,
    Money Fees,
    int SlippageBps,
    string Provider,
    string? ProviderRef);