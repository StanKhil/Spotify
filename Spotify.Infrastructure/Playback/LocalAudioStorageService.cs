namespace Spotify.Infrastructure.Playback;

public interface ILocalAudioStorageService
{
    string? GetSafeFilePath(string storageKey);
}

public sealed class LocalAudioStorageService : ILocalAudioStorageService
{
    private readonly string _rootPath;

    public LocalAudioStorageService(PlaybackOptions options)
    {
        _rootPath = Path.GetFullPath(options.LocalStorageRoot);
    }

    public string? GetSafeFilePath(string storageKey)
    {
        var trimmedKey = storageKey.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);
        var filePath = Path.GetFullPath(Path.Combine(_rootPath, trimmedKey));
        var rootWithSeparator = _rootPath.EndsWith(Path.DirectorySeparatorChar)
            ? _rootPath
            : _rootPath + Path.DirectorySeparatorChar;

        return filePath.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase)
            ? filePath
            : null;
    }
}