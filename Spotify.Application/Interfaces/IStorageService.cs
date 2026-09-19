using Spotify.Application.DTOs.Storage;

namespace Spotify.Application.Interfaces;

public interface IStorageService
{
    Task<StoredImage?> GetImageAsync(
        Guid imageId,
        CancellationToken cancellationToken = default);
}
