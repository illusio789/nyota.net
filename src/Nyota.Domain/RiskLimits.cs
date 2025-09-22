namespace Nyota.Domain;

public sealed record RiskLimits(
    decimal MaxPositionNotionalPct,
    decimal MaxAssetClassNotionalPct,
    decimal MaxDailyTurnoverPct,
    decimal MinAvgDailyVolumeUsd,
    int MaxSpreadBps,
    bool LeverageAllowed,
    bool ShortingAllowed);