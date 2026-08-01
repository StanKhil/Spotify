using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Dashboard;
using Spotify.Application.Interfaces;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class DashboardService : IDashboardService
{
    private const string AuthorRoleName = "Author";

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
}