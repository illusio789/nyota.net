using System.Text.Json;

using Nyota.Abstractions;
using Nyota.Domain;

namespace Nyota.Storage.File;

public sealed class FileRunConfigRepository : IRunConfigRepository
{
    public async Task<BacktestConfig> LoadAsync(string pathOrId, CancellationToken ct)
    {
        var json = await System.IO.File.ReadAllTextAsync(pathOrId, ct);
        return JsonSerializer.Deserialize<BacktestConfig>(json, new JsonSerializerOptions{})!;
    }
}
