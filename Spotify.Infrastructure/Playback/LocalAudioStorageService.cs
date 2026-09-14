namespace Spotify.Infrastructure.Playback;

public interface ILocalAudioStorageService
{
    string? GetSafeFilePath(string storageKey);
}

public sealed class LocalAudioStorageService : ILocalAudioStorageService
{
    private readonly string _audioRootPath;
    private readonly string _webRootPath;

    public LocalAudioStorageService(string contentRootPath, string webRootPath)
    {
        _audioRootPath = Path.GetFullPath(Path.Combine(contentRootPath, "App_Data", "audio"));
        _webRootPath = Path.GetFullPath(webRootPath);
    }

    public string? GetSafeFilePath(string storageKey)
    {
        // Supports existing public uploads while new files use private App_Data/audio.
        var isLegacyPublicAudio = storageKey.StartsWith("/uploads/audio/", StringComparison.OrdinalIgnoreCase);
        var rootPath = isLegacyPublicAudio ? _webRootPath : _audioRootPath;
        var trimmedKey = isLegacyPublicAudio
            ? storageKey.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar)
            : Path.GetFileName(storageKey);
        var filePath = Path.GetFullPath(Path.Combine(rootPath, trimmedKey));
        var rootWithSeparator = rootPath.EndsWith(Path.DirectorySeparatorChar)
            ? rootPath
            : rootPath + Path.DirectorySeparatorChar;

        return filePath.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase)
            ? filePath
            : null;
    }
}
