using Microsoft.AspNetCore.Mvc;
using Spotify.Application.DTOs.Episode;
using Spotify.Application.Interfaces;

namespace Spotify.Web.Controllers;

[ApiController]
[Route("api/admin/episodes")]
public sealed class EpisodeController : ControllerBase
{
    private readonly IEpisodeService _episodeService;

    public EpisodeController(IEpisodeService episodeService)
    {
        _episodeService = episodeService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<EpisodeResponse>>> GetEpisodes(
        [FromQuery] Guid podcastId, CancellationToken cancellationToken)
        => Ok(await _episodeService.GetEpisodesAsync(podcastId, cancellationToken));

    [HttpGet("{id}")]
    public async Task<ActionResult<EpisodeResponse>> GetEpisodeById(Guid id, CancellationToken cancellationToken)
    {
        var episode = await _episodeService.GetEpisodeByIdAsync(id, cancellationToken);
        return episode is null ? NotFound() : Ok(episode);
    }

    [HttpPost]
    public async Task<ActionResult<EpisodeResponse>> CreateEpisode(
        [FromBody] CreateEpisodeRequest request, CancellationToken cancellationToken)
    {
        var result = await _episodeService.CreateEpisodeAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            return ValidationProblem(ModelState);
        }

        return StatusCode(StatusCodes.Status201Created, result.Episode);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<EpisodeResponse>> EditEpisode(
        Guid id, [FromBody] UpdateEpisodeRequest request, CancellationToken cancellationToken)
    {
        var result = await _episodeService.EditEpisodeAsync(id, request, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            return ValidationProblem(ModelState);
        }

        return Ok(result.Episode);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEpisode(Guid id, CancellationToken cancellationToken)
    {
        var result = await _episodeService.DeleteEpisodeAsync(id, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            return ValidationProblem(ModelState);
        }

        return NoContent();
    }
}