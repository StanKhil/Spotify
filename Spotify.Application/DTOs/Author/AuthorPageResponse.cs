using Spotify.Application.DTOs.Search;

namespace Spotify.Application.DTOs.Author
{
    public sealed record AuthorPageResponse(
        Guid Id,
        string Name,
        string? Description,
        string? ImageUrl,
        long? Listeners,
        SearchPage<SearchTrackResponse> Tracks,     // Paginated tracks
        SearchPage<SearchAlbumResponse> Music,      // Paginated albums
        SearchPage<SearchTrackResponse> Interests, // Paginated recommended tracks
        SearchPage<SearchAuthorResponse> Artists   // Paginated recommended authors
    );
}
