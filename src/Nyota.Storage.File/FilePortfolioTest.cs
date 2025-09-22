// using System.Collections.Generic;
// using System.IO.Abstractions;
//
// using _fileSystem.Abstractions;
// using System.Text.Json;
//
// using Nyota.Abstractions;
// using Nyota.Domain;
//
// namespace Nyota.Storage.File
// {
//     public sealed class FilePortfolioRepository2 : IPortfolioRepository
//     {
//         private readonly string _folder;
//         private readonly IFileSystem _fileSystem;
//
//         public FilePortfolioRepository2(string folder, IFileSystem fileSystem)
//         {
//             _folder = folder;
//             _fileSystem = fileSystem;
//             _fileSystem.Directory.CreateDirectory(folder);
//         }
//
//         public async System.Threading.Tasks.Task<Portfolio> GetAsync(string id, System.Threading.CancellationToken ct)
//         {
//             var path = _fileSystem.Path.Combine(_folder, id + ".json");
//             if (!_fileSystem.File.Exists(path))
//             {
//                 var empty = new Portfolio(
//                     new Money(100000m, "USD"),
//                     new Dictionary<Symbol, object>());
//                 await SaveAsync(id, empty, ct);
//                 return empty;
//             }
//
//             var json = await _fileSystem.File.ReadAllTextAsync(path, ct);
//             return JsonSerializer.Deserialize<Portfolio>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
//         }
//
//         public async System.Threading.Tasks.Task SaveAsync(string id, Portfolio p,
//             System.Threading.CancellationToken ct)
//         {
//             var path = _fileSystem.Path.Combine(_folder, id + ".json");
//             var json = System.Text.Json.JsonSerializer.Serialize(p, new JsonSerializerOptions { WriteIndented = true });
//             await _fileSystem.File.WriteAllTextAsync(path, json, ct);
//         }
//     }
// }
