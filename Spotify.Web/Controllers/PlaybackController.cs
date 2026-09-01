using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spotify.Application.DTOs.Playback;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.User;

namespace Spotify.Web.Controllers;

[ApiController]
[Route("api/playback")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class PlaybackController : ControllerBase
{
    private readonly IPlaybackService _playbackService;
    private readonly UserManager<ApplicationUser> _userManager;

    public PlaybackController(
        IPlaybackService playbackService,
        UserManager<ApplicationUser> userManager)
    {
        _playbackService = playbackService;
        _userManager = userManager;
    }

    [HttpGet("tracks/{trackId:guid}")]
    [ProducesResponseType(typeof(TrackPlaybackResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPlayback(
        Guid trackId,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return Unauthorized();
        }

        var playback = await _playbackService.GetTrackPlaybackAsync(
            trackId,
            user.Id,
            cancellationToken);

        if (playback is null)
        {
            return NotFound();
        }

        return Ok(playback);
    }
}