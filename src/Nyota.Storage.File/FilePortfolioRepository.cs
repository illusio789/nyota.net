using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Nyota.Abstractions;
using Nyota.Domain;

namespace Nyota.Storage.File;

public sealed class FilePortfolioRepository : IPortfolioRepository
{
    private readonly string _folder;
    private readonly IFileSystem _fileSystem;

    public FilePortfolioRepository(string folder, IFileSystem fileSystem)
    {
        _folder = folder;
        _fileSystem = fileSystem;
        Directory.CreateDirectory(folder);
    }

    public async Task<Portfolio> GetAsync(string portfolioId, CancellationToken ct)
    {
        var path = Path.Combine(_folder, portfolioId + ".json");
        if (!_fileSystem.File.Exists(path))
        {
            var empty = new Portfolio(new Money(100000m, "USD"), new Dictionary<string, Money>(),
                new Dictionary<Symbol, Position>(), new Money(0, "USD"), new Money(0, "USD"));
            await SaveAsync(portfolioId, empty, ct);
            return empty;
        }

        var json = await _fileSystem.File.ReadAllTextAsync(path, ct);
        return JsonSerializer.Deserialize<Portfolio>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task SaveAsync(string portfolioId, Portfolio p, CancellationToken ct)
    {
        var path = Path.Combine(_folder, portfolioId + ".json");
        var json = JsonSerializer.Serialize(p, new JsonSerializerOptions { WriteIndented = true });
        await _fileSystem.File.WriteAllTextAsync(path, json, ct);
    }
}
