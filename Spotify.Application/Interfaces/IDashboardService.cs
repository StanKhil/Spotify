using Spotify.Application.DTOs.Dashboard;

namespace Spotify.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsResponse> GetDashboardStatsAsync(CancellationToken cancellationToken = default);
}