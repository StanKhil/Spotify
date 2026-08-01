namespace Spotify.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveAsync(
        Stream content,
        string fileName,
        string folder,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(string relativeUrl, CancellationToken cancellationToken = default);
}