namespace Nyota.Domain;

public sealed record ExecutionRules(TimeInForce DefaultTimeInForce, bool EodBatchForEtfs);