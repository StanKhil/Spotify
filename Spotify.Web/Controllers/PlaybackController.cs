using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.Content;

namespace Spotify.Web.Controllers
{
    [ApiController]
    [Route("api/playback")]
    [Authorize]
    public class PlaybackController : ControllerBase
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
        public async Task<IActionResult> GetPlayback(
            Guid trackId,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return Unauthorized();

            var userId = user.Id;

            var result = await _playbackService.GetTrackPlaybackAsync(
                trackId,
                userId,
                cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }
    }
}
