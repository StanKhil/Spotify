using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spotify.Application.DTOs.Audiobook;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.User;

namespace Spotify.Web.Controllers;

[ApiController]
[Route("api/audiobooks/actions")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class AudiobookActionController : ControllerBase
{
    private readonly IAudiobookActionService _audiobookActionService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AudiobookActionController(
        IAudiobookActionService audiobookActionService,
        UserManager<ApplicationUser> userManager)
    {
        _audiobookActionService = audiobookActionService;
        _userManager = userManager;
    }

    [HttpPost("{audiobookId:guid}/play")]
    public async Task<ActionResult<AudiobookActionResponse>> Play(
        Guid audiobookId,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized();

        var result = await _audiobookActionService.PlayAsync(audiobookId, user.Id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{audiobookId:guid}/like")]
    public async Task<ActionResult<AudiobookActionResponse>> Like(
        Guid audiobookId,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized();

        var result = await _audiobookActionService.LikeAsync(audiobookId, user.Id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{audiobookId:guid}/like")]
    public async Task<ActionResult<AudiobookActionResponse>> Unlike(
        Guid audiobookId,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized();

        var result = await _audiobookActionService.UnlikeAsync(audiobookId, user.Id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("liked/{maxPerPage:int}/{page:int}/{userId:guid}")]
    [ProducesResponseType(typeof(GetLikedAudiobooksResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GetLikedAudiobooksResult>> GetLikedAudiobooks(
        int maxPerPage,
        int page,
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (maxPerPage <= 0 || page <= 0)
            return BadRequest("maxPerPage and page must be greater than 0.");

        var result = await _audiobookActionService.GetLikedAudiobooksAsync(
            maxPerPage,
            page,
            userId,
            cancellationToken);

        return Ok(result);
    }
}
