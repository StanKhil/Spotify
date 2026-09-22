using Spotify.Application.DTOs.Search;

namespace Spotify.Application.Interfaces;

public interface ISearchService
{
    Task<SearchPage<SearchTrackResponse>> SearchTracksAsync(string query, int maxPerPage, int page, CancellationToken cancellationToken = default);
    Task<SearchPage<SearchAlbumResponse>> SearchAlbumsAsync(string query, int maxPerPage, int page, CancellationToken cancellationToken = default);
    Task<SearchPage<SearchAuthorResponse>> SearchAuthorsAsync(string query, int maxPerPage, int page, CancellationToken cancellationToken = default);
}
