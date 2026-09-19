using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Storage;
using Spotify.Application.Interfaces;
using Spotify.Infrastructure.Persistance.Context;
using Microsoft.AspNetCore.Hosting;

namespace Spotify.Infrastructure.Services;

public sealed class StorageService : IStorageService
{
    private const string ImagesRelativeDirectory = "/uploads/images/";

    private readonly ApplicationContext _context;
    private readonly string _imagesDirectoryPath;

    public StorageService(ApplicationContext context, IWebHostEnvironment environment)
    {
        _context = context;

        var webRootPath = environment.WebRootPath
            ?? Path.Combine(environment.ContentRootPath, "wwwroot");
        _imagesDirectoryPath = Path.Combine(webRootPath, "uploads", "images");
    }

    public async Task<StoredImage?> GetImageAsync(
        Guid imageId,
        CancellationToken cancellationToken = default)
    {
        var storageKey = await _context.ImageItems
            .AsNoTracking()
            .Where(x => x.Id == imageId)
            .Select(x => x.ImageList)
            .FirstOrDefaultAsync(cancellationToken);

        if (!TryGetImagePath(storageKey, out var imagePath) || !File.Exists(imagePath))
            return null;

        var stream = new FileStream(
            imagePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 64 * 1024,
            useAsync: true);

        return new StoredImage(stream, GetContentType(imagePath));
    }

    private bool TryGetImagePath(string? storageKey, out string imagePath)
    {
        imagePath = string.Empty;

        if (string.IsNullOrWhiteSpace(storageKey) ||
            !storageKey.StartsWith(ImagesRelativeDirectory, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var fileName = Path.GetFileName(storageKey);
        var expectedStorageKey = $"{ImagesRelativeDirectory}{fileName}";

        if (string.IsNullOrWhiteSpace(fileName) ||
            !string.Equals(storageKey, expectedStorageKey, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        imagePath = Path.Combine(_imagesDirectoryPath, fileName);
        return true;
    }

    private static string GetContentType(string imagePath) => Path.GetExtension(imagePath).ToLowerInvariant() switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".webp" => "image/webp",
        _ => "application/octet-stream"
    };
}
