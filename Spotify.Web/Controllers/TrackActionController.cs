using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.User;

namespace Spotify.Web.Controllers;

[ApiController]
[Route("api/tracks")]
[Authorize]
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

    [HttpPost("{trackId:guid}/play")]
    public async Task<IActionResult> Play(
        Guid trackId,
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

    [HttpPost("{trackId:guid}/like")]
    public async Task<IActionResult> Like(
        Guid trackId,
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

    [HttpDelete("{trackId:guid}/like")]
    public async Task<IActionResult> Unlike(
        Guid trackId,
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
}