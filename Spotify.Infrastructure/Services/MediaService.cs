using Spotify.Application.DTOs.Media;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.Content;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class MediaService : IMediaService
{
    private static readonly HashSet<string> AllowedAudioExtensions = new(StringComparer.OrdinalIgnoreCase) { ".mp3", ".wav", ".m4a", ".ogg" };
    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };
    private const long MaxAudioSizeBytes = 50 * 1024 * 1024; // 50 MB
    private const long MaxImageSizeBytes = 5 * 1024 * 1024;  // 5 MB

    private readonly IFileStorageService _fileStorageService;
    private readonly ApplicationContext _context;

    public MediaService(IFileStorageService fileStorageService, ApplicationContext context)
    {
        _fileStorageService = fileStorageService;
        _context = context;
    }

    public async Task<MediaUploadResult> UploadAudioAsync(
        Stream content, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(fileName);

        if (!AllowedAudioExtensions.Contains(extension))
        {
            return MediaUploadResult.Failure($"Unsupported audio format. Allowed: {string.Join(", ", AllowedAudioExtensions)}");
        }

        if (content.Length > MaxAudioSizeBytes)
        {
            return MediaUploadResult.Failure($"Audio file is too large. Max size is {MaxAudioSizeBytes / 1024 / 1024} MB.");
        }

        var url = await _fileStorageService.SaveAsync(content, fileName, "audio", cancellationToken);

        var audioItem = new AudioItem
        {
            Id = Guid.NewGuid(),
            AudioList = url
        };

        _context.AudioItems.Add(audioItem);
        await _context.SaveChangesAsync(cancellationToken);

        return MediaUploadResult.Success(audioItem.Id, url);
    }

    public async Task<MediaUploadResult> UploadImageAsync(
        Stream content, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(fileName);

        if (!AllowedImageExtensions.Contains(extension))
        {
            return MediaUploadResult.Failure($"Unsupported image format. Allowed: {string.Join(", ", AllowedImageExtensions)}");
        }

        if (content.Length > MaxImageSizeBytes)
        {
            return MediaUploadResult.Failure($"Image file is too large. Max size is {MaxImageSizeBytes / 1024 / 1024} MB.");
        }

        var url = await _fileStorageService.SaveAsync(content, fileName, "images", cancellationToken);

        var imageItem = new ImageItem
        {
            Id = Guid.NewGuid(),
            ImageList = url
        };

        _context.ImageItems.Add(imageItem);
        await _context.SaveChangesAsync(cancellationToken);

        return MediaUploadResult.Success(imageItem.Id, url);
    }
}