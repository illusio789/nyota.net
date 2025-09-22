using System.IO.Abstractions;

using Nyota.Abstractions;

namespace Nyota.Storage.File;

public sealed class FileReportSink : IReportSink
{
    private readonly string _folder;
    private readonly IFileSystem _fileSystem;

    public FileReportSink(string folder, IFileSystem fileSystem)
    {
        _folder = folder;
        _fileSystem = fileSystem;
        fileSystem.Directory.CreateDirectory(folder);
    }

    public async Task SaveAsync(string runId, byte[] bytes, string contentType, string suggestedName,
        CancellationToken ct)
    {
        var name = string.IsNullOrWhiteSpace(suggestedName) ? $"{runId}.bin" : suggestedName;
                await _fileSystem.File.WriteAllBytesAsync(_fileSystem.Path.Combine(_folder, name), bytes, ct);
    }
}
