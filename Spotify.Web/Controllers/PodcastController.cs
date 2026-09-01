using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spotify.Application.DTOs.Podcast;
using Spotify.Application.Interfaces;

namespace Spotify.Web.Controllers;

[ApiController]
[Route("api/admin/podcasts")]
[Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class PodcastController : ControllerBase
{
    private readonly IPodcastService _podcastService;

    public PodcastController(IPodcastService podcastService)
    {
        _podcastService = podcastService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<PodcastResponse>>> GetPodcasts(CancellationToken cancellationToken)
        => Ok(await _podcastService.GetPodcastsAsync(cancellationToken));

    [HttpGet("{id}")]
    public async Task<ActionResult<PodcastResponse>> GetPodcastById(Guid id, CancellationToken cancellationToken)
    {
        var podcast = await _podcastService.GetPodcastByIdAsync(id, cancellationToken);
        return podcast is null ? NotFound() : Ok(podcast);
    }

    [HttpPost]
    public async Task<ActionResult<PodcastResponse>> CreatePodcast(
        [FromBody] CreatePodcastRequest request, CancellationToken cancellationToken)
    {
        var result = await _podcastService.CreatePodcastAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            return ValidationProblem(ModelState);
        }

        return StatusCode(StatusCodes.Status201Created, result.Podcast);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PodcastResponse>> EditPodcast(
        Guid id, [FromBody] UpdatePodcastRequest request, CancellationToken cancellationToken)
    {
        var result = await _podcastService.EditPodcastAsync(id, request, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            return ValidationProblem(ModelState);
        }

        return Ok(result.Podcast);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePodcast(Guid id, CancellationToken cancellationToken)
    {
        var result = await _podcastService.DeletePodcastAsync(id, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            return ValidationProblem(ModelState);
        }

        return NoContent();
    }
}