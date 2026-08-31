using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet("subscribed/{userId:guid}")]
    public async Task<IActionResult> GetSubscribed(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
            return Unauthorized();

        var result = await _authorActionService.GetSubscribed(
            userId,
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}