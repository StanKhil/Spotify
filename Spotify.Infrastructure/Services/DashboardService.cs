using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Dashboard;
using Spotify.Application.Interfaces;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class DashboardService : IDashboardService
{
    private const string AuthorRoleName = "Author";
    private const int RecentItemsCount = 12;
    private const int TopItemsCount = 10;

    private readonly ApplicationContext _context;

    public DashboardService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<DashboardStatsResponse> GetDashboardStatsAsync(
        CancellationToken cancellationToken = default)
    {
        var totalTracks = await _context.Tracks.CountAsync(x => x.DeletedAt == null, cancellationToken);
        var totalAlbums = await _context.Albums.CountAsync(x => x.DeletedAt == null, cancellationToken);
        var totalPodcasts = await _context.Podcasts.CountAsync(cancellationToken);
        var totalAudiobooks = await _context.Audiobooks.CountAsync(x => x.DeletedAt == null, cancellationToken);
        var totalPlaylists = await _context.Playlists.CountAsync(cancellationToken);
        var totalCustomers = await _context.UserProfiles.CountAsync(x => x.DeletedAt == null, cancellationToken);
        var totalPlays = await _context.Tracks.Where(x => x.DeletedAt == null).SumAsync(x => x.PlaysNumber, cancellationToken);

        var authorRole = await _context.Roles.FirstOrDefaultAsync(x => x.Name == AuthorRoleName, cancellationToken);
        var totalAuthors = authorRole is null
            ? 0
            : await _context.UserRoles.CountAsync(x => x.RoleId == authorRole.Id, cancellationToken);

        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        var newCustomers = await _context.UserProfiles
            .CountAsync(x => x.RegisteredAt >= thirtyDaysAgo, cancellationToken);

        return new DashboardStatsResponse(
            totalTracks, totalAlbums, totalPodcasts, totalAudiobooks,
            totalPlaylists, totalCustomers, totalAuthors, totalPlays, newCustomers);
    }

    public async Task<LibraryOverviewResponse> GetLibraryOverviewAsync(
        CancellationToken cancellationToken = default)
    {
        var recentTracks = await _context.Tracks
            .Where(x => x.DeletedAt == null)
            .Include(x => x.ImageItem)
            .Include(x => x.Album)
            .OrderByDescending(x => x.CreatedAt)
            .Take(RecentItemsCount)
            .Select(x => new LibraryTrackSummary(
                x.Id, x.Name, x.ImageItem != null ? x.ImageItem.ImageList : null,
                x.DurationSeconds, x.PlaysNumber,
                x.AlbumId, x.Album != null ? x.Album.Name : null))
            .ToListAsync(cancellationToken);

        var recentAlbums = await _context.Albums
            .Where(x => x.DeletedAt == null)
            .Include(x => x.ImageItem)
            .OrderByDescending(x => x.CreatedAt)
            .Take(RecentItemsCount)
            .Select(x => new LibraryAlbumSummary(
                x.Id, x.Name, x.ImageItem != null ? x.ImageItem.ImageList : null,
                x.Tracks.Count(t => t.DeletedAt == null)))
            .ToListAsync(cancellationToken);

        var topTracks = await _context.Tracks
            .Where(x => x.DeletedAt == null)
            .Include(x => x.ImageItem)
            .Include(x => x.Album)
            .OrderByDescending(x => x.PlaysNumber)
            .Take(TopItemsCount)
            .Select(x => new LibraryTrackSummary(
                x.Id, x.Name, x.ImageItem != null ? x.ImageItem.ImageList : null,
                x.DurationSeconds, x.PlaysNumber,
                x.AlbumId, x.Album != null ? x.Album.Name : null))
            .ToListAsync(cancellationToken);

        var topAlbums = await _context.Albums
            .Where(x => x.DeletedAt == null)
            .Include(x => x.ImageItem)
            .Include(x => x.Tracks)
            .Select(x => new
            {
                Album = x,
                TotalPlays = x.Tracks.Where(t => t.DeletedAt == null).Sum(t => (long?)t.PlaysNumber) ?? 0
            })
            .OrderByDescending(x => x.TotalPlays)
            .Take(TopItemsCount)
            .Select(x => new LibraryAlbumSummary(
                x.Album.Id, x.Album.Name,
                x.Album.ImageItem != null ? x.Album.ImageItem.ImageList : null,
                x.Album.Tracks.Count(t => t.DeletedAt == null)))
            .ToListAsync(cancellationToken);

        var topGenres = await _context.Genres
            .Select(g => new
            {
                Genre = g,
                TrackCount = _context.Tracks.Count(t => t.DeletedAt == null && t.GenreId == g.Id)
            })
            .OrderByDescending(x => x.TrackCount)
            .Take(TopItemsCount)
            .Select(x => new LibraryGenreSummary(x.Genre.Id, x.Genre.Name, x.TrackCount))
            .ToListAsync(cancellationToken);

        return new LibraryOverviewResponse(recentTracks, recentAlbums, topTracks, topAlbums, topGenres);
    }

    public async Task<IReadOnlyCollection<LibraryTrackSummary>> GetAlbumTracksAsync(
        Guid albumId, CancellationToken cancellationToken = default)
    {
        return await _context.Tracks
            .Where(x => x.DeletedAt == null && x.AlbumId == albumId)
            .Include(x => x.ImageItem)
            .Include(x => x.Album)
            .OrderBy(x => x.Name)
            .Select(x => new LibraryTrackSummary(
                x.Id, x.Name, x.ImageItem != null ? x.ImageItem.ImageList : null,
                x.DurationSeconds, x.PlaysNumber,
                x.AlbumId, x.Album != null ? x.Album.Name : null))
            .ToListAsync(cancellationToken);
    }
}