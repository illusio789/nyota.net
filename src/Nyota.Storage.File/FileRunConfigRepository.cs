using System.IO.Abstractions;
using System.Text.Json;

using Nyota.Abstractions;
using Nyota.Domain;

namespace Nyota.Storage.File;

public sealed class FileRunConfigRepository : IRunConfigRepository
{
    private readonly IFileSystem _fileSystem;

    public FileRunConfigRepository(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }


    public async Task<BacktestConfig> LoadAsync(string pathOrId, CancellationToken ct)
    {
        var json = await _fileSystem.File.ReadAllTextAsync(pathOrId, ct);
        return JsonSerializer.Deserialize<BacktestConfig>(json, new JsonSerializerOptions{})!;
    }
}
