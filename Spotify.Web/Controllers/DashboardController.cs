using Microsoft.AspNetCore.Mvc;
using Spotify.Application.DTOs.Dashboard;
using Spotify.Application.Interfaces;

namespace Spotify.Web.Controllers;

[ApiController]
[Route("api/admin/dashboard")]
public sealed class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("stats")]
    public async Task<ActionResult<DashboardStatsResponse>> GetDashboardStats(CancellationToken cancellationToken)
        => Ok(await _dashboardService.GetDashboardStatsAsync(cancellationToken));
}