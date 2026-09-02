using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spotify.Application.DTOs.Album;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.User;

namespace Spotify.Web.Controllers
{
    [ApiController]
    [Route("api/albums/actions")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AlbumActionController : ControllerBase
    {
        private readonly IAlbumActionService _albumActionService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AlbumActionController(
            IAlbumActionService albumActionService,
            UserManager<ApplicationUser> userManager)
        {
            _albumActionService = albumActionService;
            _userManager = userManager;
        }

        [HttpGet("liked/{maxPerPage}/{page}/{userId:guid}")]
        [ProducesResponseType(typeof(GetLikedAlbumsResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GetLikedAlbumsResult>> GetLikedAlbums(
            int maxPerPage,
            int page,
            Guid userId,
            CancellationToken cancellationToken)
        {
            if (maxPerPage <= 0 || page <= 0)
                return BadRequest("maxPerPage and page must be greater than 0.");

            var result = await _albumActionService.GetLikedAlbumsAsync(
                maxPerPage,
                page,
                userId,
                cancellationToken);
            return Ok(result);
        }

        [HttpPost("{albumId}/like")]
        public async Task<IActionResult> Like(
            string albumId,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return Unauthorized();

            var result = await _albumActionService.LikeAsync(
                albumId,
                user.Id,
                cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete("{albumId}/like")]
        public async Task<IActionResult> Unlike(
            string albumId,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return Unauthorized();

            var result = await _albumActionService.UnlikeAsync(
                albumId,
                user.Id,
                cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }
    }
}
