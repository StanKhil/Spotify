using Spotify.Domain.Entities.Content;
namespace Spotify.Application.Interfaces
{
    public interface IFromJamendoToLocalService
    {
        Task<Album?> GetOrCreateJamendoAlbumAsync(
            string jamendoAlbumId,
            CancellationToken cancellationToken);

        Task<Track?> GetOrCreateJamendoTrackAsync(
            string jamendoTrackId,
            CancellationToken cancellationToken);

        Task<Author?> GetOrCreateJamendoAuthorAsync(
            string jamendoAuthorId,
            CancellationToken cancellationToken);

        bool IsJamendoId(string trackId);
    }
}
