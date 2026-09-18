using Spotify.Application.DTOs.Media;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.Content;
using Spotify.Domain.Enumerations;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class MediaService : IMediaService
{
    private static readonly HashSet<string> AllowedAudioExtensions = new(StringComparer.OrdinalIgnoreCase) { ".mp3", ".wav", ".m4a", ".ogg" };
    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };
    private const long MaxAudioSizeBytes = 50 * 1024 * 1024; // 50 MB
    private const long MaxImageSizeBytes = 50 * 1024 * 1024;  // 50 MB

    private readonly IFileStorageService _fileStorageService;
    private readonly ApplicationContext _context;

    public MediaService(IFileStorageService fileStorageService, ApplicationContext context)
    {
        _fileStorageService = fileStorageService;
        _context = context;
    }

    public async Task<MediaUploadResult> UploadAudioAsync(
        Stream content,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
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

        var audioItemId = Guid.NewGuid();
        var storageKey = await _fileStorageService.SaveAsync(
            content,
            fileName,
            audioItemId,
            "audio",
            cancellationToken);
        var now = DateTime.UtcNow;

        var audioItem = new AudioItem
        {
            Id = audioItemId,
            Provider = AudioProvider.LocalStorage,
            StorageKey = storageKey,
            ContentType = contentType,
            BitrateKbps = null,
            LicenseUrl = null,
            IsDownloadAllowed = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        _context.AudioItems.Add(audioItem);
        await _context.SaveChangesAsync(cancellationToken);

        return MediaUploadResult.Success(audioItem.Id, storageKey);
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

        var imageItemId = Guid.NewGuid();
        var url = await _fileStorageService.SaveAsync(
            content,
            fileName,
            imageItemId,
            "images",
            cancellationToken);

        var imageItem = new ImageItem
        {
            Id = imageItemId,
            ImageList = url
        };

        _context.ImageItems.Add(imageItem);
        await _context.SaveChangesAsync(cancellationToken);

        return MediaUploadResult.Success(imageItem.Id, url);
    }
}
