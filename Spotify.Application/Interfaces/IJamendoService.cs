using Spotify.Application.DTOs.Jamendo;

namespace Spotify.Application.Interfaces
{
    public interface IJamendoService
    {
        Task<IReadOnlyCollection<JamendoTrackDto>> SearchTracksAsync(
            string query,
            int maxPerPage = 20,
            int page = 1,
            CancellationToken cancellationToken = default);

        Task<JamendoTrackDto?> GetTrackAsync(
            string trackId,
            CancellationToken cancellationToken = default);

        Task<string?> GetTrackStreamUrlAsync(
            string trackId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<JamendoAlbumDto>> SearchAlbumsAsync(
            string query,
            int maxPerPage = 20,
            int page = 1,
            CancellationToken cancellationToken = default);

        Task<JamendoAlbumTrackDto?> GetAlbumAsync(
            string albumId,
            CancellationToken cancellationToken = default);

        Task<JamendoAuthorDto?> GetAuthorAsync(
            string authorId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<JamendoAuthorDto>> SearchAuthorsAsync(
            string query,
            int maxPerPage = 20,
            int page = 1,
            CancellationToken cancellationToken = default);

        Task<JamendoAuthorTracksDto?> GetTracksByAuthorAsync(
            string authorId,
            int maxPerPage = 20,
            int page = 1,
            CancellationToken cancellationToken = default);

        Task<JamendoAuthorAlbumsDto?> GetAlbumsByAuthorAsync(
            string authorId,
            int maxPerPage = 20,
            int page = 1,
            CancellationToken cancellationToken = default);
    }
}
