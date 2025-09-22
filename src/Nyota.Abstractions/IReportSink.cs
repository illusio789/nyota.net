namespace Nyota.Abstractions;

public interface IReportSink
{
    Task SaveAsync(string runId, byte[] bytes, string contentType, string suggestedName, CancellationToken ct);
}