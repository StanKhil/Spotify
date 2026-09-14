namespace Spotify.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveAsync(
        Stream content,
        string fileName,
        Guid itemId,
        string folder,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(string relativeUrl, CancellationToken cancellationToken = default);
}