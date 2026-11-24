namespace VizsgaremekBackend.Interfaces;

public interface IFileService
{
    public Task<(Stream, string)> GetAsync(string key, CancellationToken ct = default);
    
    public Task PostAsync(string key,
        Stream value,
        string contentType,
        CancellationToken ct = default);
    
    public Task DeleteAsync(string key, CancellationToken ct = default);
}