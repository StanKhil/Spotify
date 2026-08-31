using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spotify.Application.DTOs.Track;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.User;

namespace Spotify.Web.Controllers;

[ApiController]
[Route("api/authors/actions")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class AuthorActionController : ControllerBase
{
    private readonly IAuthorActionService _authorActionService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthorActionController(
        IAuthorActionService authorActionService,
        UserManager<ApplicationUser> userManager)
    {
        _authorActionService = authorActionService;
        _userManager = userManager;
    }

    [HttpPost("{authorId:guid}/subscribe")]
    public async Task<IActionResult> Subscribe(
        Guid authorId,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
            return Unauthorized();

        var result = await _authorActionService.SubscribeAsync(
            authorId,
            user.Id,
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{authorId:guid}/subscribe")]
    public async Task<IActionResult> Unsubscribe(
        Guid authorId,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
            return Unauthorized();

        var result = await _authorActionService.UnsubscribeAsync(
            authorId,
            user.Id,
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("subscribed/{maxPerPage}/{page}/{userId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<TrackResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSubscribed(
        int maxPerPage,
        int page,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
            return Unauthorized();

        var result = await _authorActionService.GetSubscribed(
            maxPerPage,
            page,
            userId,
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}