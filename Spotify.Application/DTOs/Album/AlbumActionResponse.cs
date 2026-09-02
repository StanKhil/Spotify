namespace Spotify.Application.DTOs.Album
{
    public sealed record AlbumActionResponse(
        Guid AlbumId,
        bool IsLiked);

}
