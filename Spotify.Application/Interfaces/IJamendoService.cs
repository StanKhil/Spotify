using Spotify.Application.DTOs.Jamendo;

namespace Spotify.Application.Interfaces
{
    public interface IJamendoService
    {
        Task<IReadOnlyCollection<JamendoTrackDto>> SearchTracksAsync(
            string query,
            int limit = 20,
            CancellationToken cancellationToken = default);

        Task<JamendoTrackDto?> GetTrackAsync(
            string trackId,
            CancellationToken cancellationToken = default);

        Task<string?> GetTrackStreamUrlAsync(
            string trackId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<JamendoAlbumDto>> SearchAlbumsAsync(
            string query,
            int limit = 20,
            CancellationToken cancellationToken = default);

        Task<JamendoAlbumTrackDto?> GetAlbumAsync(
            string albumId,
            CancellationToken cancellationToken = default);
    }
}
