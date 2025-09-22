namespace Nyota.Domain;

public sealed record StrategyConfig(string Id, string Type, IReadOnlyDictionary<string, object> Params);