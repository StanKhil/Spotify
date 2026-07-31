using Microsoft.AspNetCore.Mvc;
using Spotify.Application.DTOs.Plugin;
using Spotify.Application.Interfaces;

namespace Spotify.Web.Controllers;

[ApiController]
[Route("api/admin/plugins")]
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
}