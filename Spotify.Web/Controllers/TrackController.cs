using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spotify.Application.DTOs.Track;
using Spotify.Application.Interfaces;

namespace Spotify.Web.Controllers;

[ApiController]
[Route("api/admin/tracks")]
[Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class TrackController : ControllerBase
{
    private readonly ITrackService _trackService;

    public TrackController(ITrackService trackService)
    {
        _trackService = trackService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TrackResponse>>> GetTracks(CancellationToken cancellationToken)
        => Ok(await _trackService.GetTracksAsync(cancellationToken));

    [HttpGet("{id}")]
    public async Task<ActionResult<TrackResponse>> GetTrackById(Guid id, CancellationToken cancellationToken)
    {
        var track = await _trackService.GetTrackByIdAsync(id, cancellationToken);
        return track is null ? NotFound() : Ok(track);
    }

    [HttpPost]
    public async Task<ActionResult<TrackResponse>> CreateTrack(
        [FromBody] CreateTrackRequest request, CancellationToken cancellationToken)
    {
        var result = await _trackService.CreateTrackAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            return ValidationProblem(ModelState);
        }

        return StatusCode(StatusCodes.Status201Created, result.Track);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TrackResponse>> EditTrack(
        Guid id, [FromBody] UpdateTrackRequest request, CancellationToken cancellationToken)
    {
        var result = await _trackService.EditTrackAsync(id, request, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            return ValidationProblem(ModelState);
        }

        return Ok(result.Track);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTrack(Guid id, CancellationToken cancellationToken)
    {
        var result = await _trackService.DeleteTrackAsync(id, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            return ValidationProblem(ModelState);
        }

        return NoContent();
    }

    [HttpPost("batch-delete")]
    public async Task<IActionResult> BatchDeleteTracks(
        [FromBody] BatchDeleteTracksRequest request, CancellationToken cancellationToken)
    {
        var result = await _trackService.BatchDeleteTracksAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            return ValidationProblem(ModelState);
        }

        return Ok(new { deletedCount = result.DeletedCount });
    }
}