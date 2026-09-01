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

        bool IsJamendoId(string trackId);
    }
}
