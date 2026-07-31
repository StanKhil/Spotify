using Microsoft.AspNetCore.Mvc;
using Spotify.Application.DTOs.SystemSettings;
using Spotify.Application.Interfaces;

namespace Spotify.Web.Controllers;

[ApiController]
[Route("api/admin/settings")]
public sealed class SystemSettingsController : ControllerBase
{
    private readonly ISystemSettingsService _settingsService;

    public SystemSettingsController(ISystemSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [HttpGet]
    public async Task<ActionResult<SystemSettingsResponse>> GetSystemSettings(CancellationToken cancellationToken)
        => Ok(await _settingsService.GetSystemSettingsAsync(cancellationToken));

    [HttpPut]
    public async Task<ActionResult<SystemSettingsResponse>> UpdateSystemSettings(
        [FromBody] UpdateSystemSettingsRequest request, CancellationToken cancellationToken)
        => Ok(await _settingsService.UpdateSystemSettingsAsync(request, cancellationToken));
}