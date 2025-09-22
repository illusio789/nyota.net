using System.Text.Json;

using Nyota.Abstractions;
using Nyota.Core;

namespace Nyota.Storage.File;

public sealed class FilePortfolioRepository : IPortfolioRepository
{
    private readonly string _folder;
    public FilePortfolioRepository(string folder){ _folder = folder; Directory.CreateDirectory(folder); }
    public async Task<Portfolio> GetAsync(string portfolioId, CancellationToken ct)
    {
        var path = Path.Combine(_folder, portfolioId + ".json");
        if (!System.IO.File.Exists(path))
        {
            var empty = new Portfolio(new Money(100000m, "USD"), new Dictionary<string, Money>(), new Dictionary<Symbol, Position>(), new Money(0,"USD"), new Money(0,"USD"));
            await SaveAsync(portfolioId, empty, ct);
            return empty;
        }
        var json = await System.IO.File.ReadAllTextAsync(path, ct);
        return JsonSerializer.Deserialize<Portfolio>(json, new JsonSerializerOptions{PropertyNameCaseInsensitive=true})!;
    }
    public async Task SaveAsync(string portfolioId, Portfolio p, CancellationToken ct)
    {
        var path = Path.Combine(_folder, portfolioId + ".json");
        var json = JsonSerializer.Serialize(p, new JsonSerializerOptions{WriteIndented=true});
        await System.IO.File.WriteAllTextAsync(path, json, ct);
    }
}