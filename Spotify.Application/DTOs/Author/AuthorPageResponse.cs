using Spotify.Application.DTOs.Search;

namespace Spotify.Application.DTOs.Author
{
    public sealed record AuthorPageResponse(
        Guid AuthorId,
        string Name,
        string? Description,
        string? AvatarImage,
        SearchPage<SearchTrackResponse> Tracks,       // Paginated tracks by author
        SearchPage<SearchAlbumResponse> Music,        // Paginated albums by author
        IReadOnlyList<SearchTrackResponse> Interests,  // Recommended tracks (random)
        IReadOnlyList<SearchAuthorResponse> Artists    // Recommended authors (random)
    );
}
