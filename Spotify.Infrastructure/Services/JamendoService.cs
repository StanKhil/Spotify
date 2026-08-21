using Spotify.Application.Interfaces;
using Spotify.Infrastructure.Playback;
using Spotify.Application.DTOs.Jamendo;

namespace Spotify.Infrastructure.Services
{
    public sealed class JamendoService : IJamendoService
    {
        private readonly JamendoApiClient _client;

        public JamendoService(JamendoApiClient client)
        {
            _client = client;
        }

        public Task<IReadOnlyCollection<JamendoTrackDto>> SearchTracksAsync(
            string query,
            int limit = 20,
            CancellationToken cancellationToken = default)
            => _client.SearchTracksAsync(query, limit, cancellationToken);

        public Task<JamendoTrackDto?> GetTrackAsync(
            string trackId,
            CancellationToken cancellationToken = default)
            => _client.GetTrackAsync(trackId, cancellationToken);

        public Task<string?> GetTrackStreamUrlAsync(
            string trackId,
            CancellationToken cancellationToken = default)
            => _client.GetTrackStreamUrlAsync(trackId, cancellationToken);

        public Task<IReadOnlyCollection<JamendoAlbumDto>> SearchAlbumsAsync(
            string query,
            int limit = 20,
            CancellationToken cancellationToken = default)
            => _client.SearchAlbumsAsync(query, limit, cancellationToken);

        public Task<JamendoAlbumTrackDto?> GetAlbumAsync(
            string albumId,
            CancellationToken cancellationToken = default)
            => _client.GetAlbumAsync(albumId, cancellationToken);
    }
}
