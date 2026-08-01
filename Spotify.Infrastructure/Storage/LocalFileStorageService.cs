using Spotify.Application.Interfaces;

namespace Spotify.Infrastructure.Storage;

public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly string _webRootPath;

    public LocalFileStorageService(string webRootPath)
    {
        _webRootPath = webRootPath;
    }

    public async Task<string> SaveAsync(
        Stream content,
        string fileName,
        string folder,
        CancellationToken cancellationToken = default)
    {
        var safeFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        var folderPath = Path.Combine(_webRootPath, "uploads", folder);

        Directory.CreateDirectory(folderPath);

        var fullPath = Path.Combine(folderPath, safeFileName);

        await using (var fileStream = File.Create(fullPath))
        {
            await content.CopyToAsync(fileStream, cancellationToken);
        }

        return $"/uploads/{folder}/{safeFileName}";
    }

    public Task DeleteAsync(string relativeUrl, CancellationToken cancellationToken = default)
    {
        var relativePath = relativeUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_webRootPath, relativePath.Replace($"uploads{Path.DirectorySeparatorChar}", "uploads" + Path.DirectorySeparatorChar));

        var fullFsPath = Path.Combine(_webRootPath, relativeUrl.TrimStart('/'));

        if (File.Exists(fullFsPath))
        {
            File.Delete(fullFsPath);
        }

        return Task.CompletedTask;
    }
}