using Spotify.Application.Interfaces;

namespace Spotify.Infrastructure.Storage;

public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly string _webRootPath;
    private readonly string _audioRootPath;

    public LocalFileStorageService(string webRootPath, string contentRootPath)
    {
        _webRootPath = webRootPath;
        _audioRootPath = Path.Combine(contentRootPath, "App_Data", "audio");
    }

    public async Task<string> SaveAsync(
        Stream content,
        string fileName,
        Guid itemId,
        string folder,
        CancellationToken cancellationToken = default)
    {
        var safeFileName = $"{itemId}{Path.GetExtension(fileName)}";
        if (string.Equals(folder, "audio", StringComparison.OrdinalIgnoreCase))
        {
            Directory.CreateDirectory(_audioRootPath);
            var audioPath = Path.Combine(_audioRootPath, safeFileName);

            await using var audioStream = File.Create(audioPath);
            await content.CopyToAsync(audioStream, cancellationToken);

            return safeFileName;
        }

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
        var isLegacyPublicAudio = relativeUrl.StartsWith("/uploads/audio/", StringComparison.OrdinalIgnoreCase);
        var fullFsPath = isLegacyPublicAudio
            ? Path.Combine(_webRootPath, relativeUrl.TrimStart('/'))
            : Path.Combine(_audioRootPath, Path.GetFileName(relativeUrl));

        if (File.Exists(fullFsPath))
        {
            File.Delete(fullFsPath);
        }

        return Task.CompletedTask;
    }
}
