namespace Spotify.Application.DTOs.Dashboard;

public sealed record LibraryTrackSummary(
    Guid Id,
    string Name,
    string? ImageUrl,
    int DurationSeconds,
    long PlaysNumber,
    Guid? AlbumId,
    string? AlbumName);

public sealed record LibraryAlbumSummary(
    Guid Id,
    string Name,
    string? ImageUrl,
    int TrackCount);

public sealed record LibraryGenreSummary(
    string Id,
    string Name,
    int TrackCount);

public sealed record LibraryOverviewResponse(
    IReadOnlyCollection<LibraryTrackSummary> RecentTracks,
    IReadOnlyCollection<LibraryAlbumSummary> RecentAlbums,
    IReadOnlyCollection<LibraryTrackSummary> TopTracks,
    IReadOnlyCollection<LibraryAlbumSummary> TopAlbums,
    IReadOnlyCollection<LibraryGenreSummary> TopGenres);