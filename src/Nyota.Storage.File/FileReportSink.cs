using Nyota.Abstractions;

namespace Nyota.Storage.File;

public sealed class FileReportSink : IReportSink
{
    private readonly string _folder;
    public FileReportSink(string folder){ _folder = folder; Directory.CreateDirectory(folder); }
    public async Task SaveAsync(string runId, byte[] bytes, string contentType, string suggestedName, CancellationToken ct)
    {
        var name = string.IsNullOrWhiteSpace(suggestedName) ? $"{runId}.bin" : suggestedName;
        await System.IO.File.WriteAllBytesAsync(Path.Combine(_folder, name), bytes, ct);
    }
}