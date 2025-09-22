namespace Nyota.Core;

public sealed record ExecutionRules(TimeInForce DefaultTimeInForce, bool EodBatchForEtfs);