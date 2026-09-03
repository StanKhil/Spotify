using Spotify.Application.DTOs.Jamendo;

namespace Spotify.Application.Interfaces
{
    public interface IJamendoService
    {
        Task<IReadOnlyCollection<JamendoTrackDto>> SearchTracksAsync(
    string query,
    int offset = 0,
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

        Task<JamendoAuthorDto?> GetAuthorAsync(
            string authorId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<JamendoAuthorDto>> SearchAuthorsAsync(
            string query,
            int limit = 20,
            CancellationToken cancellationToken = default);

        Task<JamendoAuthorTracksDto?> GetTracksByAuthorAsync(
            string authorId,
            int limit = 50,
            CancellationToken cancellationToken = default);

        Task<JamendoAuthorAlbumsDto?> GetAlbumsByAuthorAsync(
            string authorId,
            int limit = 50,
            CancellationToken cancellationToken = default);
    }
}
