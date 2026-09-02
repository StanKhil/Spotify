using Spotify.Application.DTOs.Album;

namespace Spotify.Application.Interfaces
{
    public interface IAlbumActionService
    {
        Task<AlbumActionResult?> LikeAsync(
            string albumId,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<AlbumActionResult?> UnlikeAsync(
            string albumId,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<GetLikedAlbumsResult> GetLikedAlbumsAsync(int maxPerPage, int page, Guid userId, CancellationToken cancellationToken = default);
    }
}
