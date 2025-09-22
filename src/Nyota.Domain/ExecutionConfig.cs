namespace Nyota.Domain;

public sealed record ExecutionConfig(string Provider, string SlippageModel, string FeesModel);