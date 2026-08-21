namespace Spotify.Application.DTOs.Jamendo
{
    public sealed record JamendoTrackDto(
        string Id,
        string Name,
        string ArtistName,
        string ArtistId,
        string AlbumName,
        string AlbumId,
        int DurationSeconds,
        string AudioUrl,
        string ImageUrl,
        bool IsExplicit,
        string Provider);
}
