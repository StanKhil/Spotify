using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spotify.Application.DTOs.Track;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.User;
using Spotify.Infrastructure.Services;

namespace Spotify.Web.Controllers;

[ApiController]
[Route("api/tracks/actions")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class TrackActionController : ControllerBase
{
    private readonly ITrackActionService _trackActionService;
    private readonly UserManager<ApplicationUser> _userManager;

    public TrackActionController(
        ITrackActionService trackActionService,
        UserManager<ApplicationUser> userManager)
    {
        _trackActionService = trackActionService;
        _userManager = userManager;
    }

    [HttpPost("{trackId}/play")]
    public async Task<IActionResult> Play(
        string trackId,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
            return Unauthorized();

        var result = await _trackActionService.PlayAsync(
            trackId,
            user.Id,
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost("{trackId}/like")]
    public async Task<IActionResult> Like(
        string trackId,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
            return Unauthorized();

        var result = await _trackActionService.LikeAsync(
            trackId,
            user.Id,
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{trackId}/like")]
    public async Task<IActionResult> Unlike(
        string trackId,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
            return Unauthorized();

        var result = await _trackActionService.UnlikeAsync(
            trackId,
            user.Id,
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("liked/{maxPerPage}/{page}/{userId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<TrackResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<TrackResponse>>> GetLikedTracks(
        int maxPerPage, int page, Guid userId, CancellationToken cancellationToken)
    {
        var tracks = await _trackActionService.GetLikedTracksAsync(maxPerPage, page, userId, cancellationToken);
        return Ok(tracks);
    }

}