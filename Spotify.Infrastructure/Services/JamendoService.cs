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
            int maxPerPage = 20,
            int page = 1,   
            CancellationToken cancellationToken = default)
    => _client.SearchTracksAsync(
        query,
        maxPerPage,
        page,
        cancellationToken);

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
            int maxPerPage = 20,
            int page = 1,
            CancellationToken cancellationToken = default)
            => _client.SearchAlbumsAsync(query, maxPerPage, page, cancellationToken);

        public Task<JamendoAlbumTrackDto?> GetAlbumAsync(
            string albumId,
            CancellationToken cancellationToken = default)
            => _client.GetAlbumAsync(albumId, cancellationToken);

        public Task<JamendoAuthorDto?> GetAuthorAsync(
            string authorId,
            CancellationToken cancellationToken = default)
            => _client.GetAuthorAsync(authorId, cancellationToken);

        public Task<IReadOnlyCollection<JamendoAuthorDto>> SearchAuthorsAsync(
            string query,
            int maxPerPage = 20,
            int page = 1,
            CancellationToken cancellationToken = default)
            => _client.SearchAuthorsAsync(query, maxPerPage, page, cancellationToken);

        public Task<JamendoAuthorTracksDto?> GetTracksByAuthorAsync(
            string authorId,
            int maxPerPage = 20,
            int page = 1,
            CancellationToken cancellationToken = default)
            => _client.GetTracksByAuthorAsync(authorId, maxPerPage, page, cancellationToken);

        public Task<JamendoAuthorAlbumsDto?> GetAlbumsByAuthorAsync(
            string authorId,
            int maxPerPage = 20,
            int page = 1,
            CancellationToken cancellationToken = default)
            => _client.GetAlbumsByAuthorAsync(authorId, maxPerPage, page, cancellationToken);
    }
}
