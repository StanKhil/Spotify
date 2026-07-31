using Spotify.Application.DTOs.Album;

namespace Spotify.Application.Interfaces;

public interface IAlbumService
{
    Task<IReadOnlyCollection<AlbumResponse>> GetAlbumsAsync(
        CancellationToken cancellationToken = default);

    Task<AlbumResponse?> GetAlbumByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<CreateAlbumResult> CreateAlbumAsync(
        CreateAlbumRequest request,
        CancellationToken cancellationToken = default);

    Task<UpdateAlbumResult> EditAlbumAsync(
        Guid id,
        UpdateAlbumRequest request,
        CancellationToken cancellationToken = default);

    Task<DeleteAlbumResult> DeleteAlbumAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}