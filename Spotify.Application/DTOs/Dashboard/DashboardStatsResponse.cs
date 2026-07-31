namespace Spotify.Application.DTOs.Dashboard;

public sealed record DashboardStatsResponse(
    int TotalTracks,
    int TotalAlbums,
    int TotalPodcasts,
    int TotalAudiobooks,
    int TotalPlaylists,
    int TotalCustomers,
    int TotalAuthors,
    long TotalPlays,
    int NewCustomersLast30Days);