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
//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

    [HttpGet("{audioContentId:guid}")]
    [HttpGet("tracks/{audioContentId:guid}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPlayback(
        Guid audioContentId,
        CancellationToken cancellationToken)
    {
        //var user = await _userManager.GetUserAsync(User);
        //
        //if (user is null)
        //{
        //    return Unauthorized();
        //}

        var user = await _userManager.GetUserAsync(User);

        var playback = await _playbackService.GetAudioContentPlaybackAsync(
            audioContentId,
            user?.Id,
            cancellationToken);

        return playback is null
            ? NotFound()
            : Content(playback, "text/plain");
    }

    [HttpGet("{audioContentId:guid}/admin")]
    [HttpGet("tracks/{audioContentId:guid}/admin")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(AudioContentPlaybackResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPlaybackForAdmin(
        Guid audioContentId,
        [FromQuery] bool asUser,
        CancellationToken cancellationToken)
    {
        Guid? userId = null;

        if (asUser)
        {
            var user = await _userManager.GetUserAsync(User);
            userId = user?.Id;
        }

        var playback = await _playbackService.GetAudioContentPlaybackForAdminAsync(
            audioContentId,
            asUser,
            userId,
            cancellationToken);

        return playback is null ? NotFound() : Ok(playback);
    }

    [HttpPost("tracks/{trackId:guid}/plays")]
    public async Task<IActionResult> RegisterPlay(Guid trackId, CancellationToken cancellationToken)
    {
        var registered = await _playbackService.RegisterPlayAsync(trackId, cancellationToken);
        return registered ? NoContent() : NotFound();
    }
}
