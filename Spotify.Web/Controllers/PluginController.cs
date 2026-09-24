using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spotify.Application.DTOs.Plugin;
using Spotify.Application.Interfaces;

namespace Spotify.Web.Controllers;

[ApiController]
[Route("api/admin/plugins")]
[Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class PluginController : ControllerBase
{
    private readonly IPluginService _pluginService;

    public PluginController(IPluginService pluginService)
    {
        _pluginService = pluginService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<PluginResponse>>> GetPlugins(CancellationToken cancellationToken)
        => Ok(await _pluginService.GetPluginsAsync(cancellationToken));

    [HttpPost]
    public async Task<ActionResult<PluginResponse>> CreatePlugin(
        [FromBody] CreatePluginRequest request, CancellationToken cancellationToken)
    {
        var result = await _pluginService.CreatePluginAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPost("{id}/toggle")]
    public async Task<ActionResult<PluginResponse>> TogglePlugin(
        Guid id, [FromBody] TogglePluginRequest request, CancellationToken cancellationToken)
    {
        var result = await _pluginService.TogglePluginAsync(id, request, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("{id}/settings")]
    public async Task<ActionResult<PluginResponse>> UpdatePluginSettings(
        Guid id, [FromBody] UpdatePluginSettingsRequest request, CancellationToken cancellationToken)
    {
        var result = await _pluginService.UpdatePluginSettingsAsync(id, request, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlugin(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _pluginService.DeletePluginAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}