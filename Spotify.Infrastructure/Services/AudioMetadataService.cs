using Microsoft.AspNetCore.Hosting;
using Spotify.Application.Interfaces;
using TagLib;

public sealed class AudioMetadataService : IAudioMetadataService
{
    private readonly IWebHostEnvironment _environment;

    public AudioMetadataService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public Task<int> GetDurationSecondsAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        var relativePath = storageKey.TrimStart('/')
            .Replace('/', Path.DirectorySeparatorChar);

        var filePath = Path.Combine(
            _environment.WebRootPath,
            relativePath);

        if (!System.IO.File.Exists(filePath))
        {
            throw new FileNotFoundException(
                "Audio file was not found.",
                filePath);
        }

        using var file = TagLib.File.Create(filePath);

        var duration = file.Properties.Duration;

        return Task.FromResult(
            (int)Math.Round(duration.TotalSeconds));
    }
}