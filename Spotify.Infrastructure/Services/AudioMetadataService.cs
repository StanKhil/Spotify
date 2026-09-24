using Microsoft.AspNetCore.Hosting;
using Spotify.Application.Interfaces;
using TagLib;

namespace Spotify.Infrastructure.Services;

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
        var audioRootPath = Path.Combine(
            _environment.ContentRootPath,
            "App_Data",
            "audio");

        var fileName = storageKey.TrimStart('/')
            .Replace('/', Path.DirectorySeparatorChar);

        var filePath = Path.Combine(audioRootPath, fileName);

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