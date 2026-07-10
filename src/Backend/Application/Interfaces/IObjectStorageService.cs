namespace Application.Interfaces;

public interface IObjectStorageService
{
    Task PutAsync(string objectName, Stream content, long size, string contentType, CancellationToken ct);
    Task<Stream> GetAsync(string objectName, CancellationToken ct);
    Task DeleteAsync(string objectName, CancellationToken ct);
}
