using Microsoft.AspNetCore.Mvc;
using Spotify.Application.Interfaces;

namespace Spotify.Web.Controllers
{
    [ApiController]
    [Route("api/jamendo")]
    public sealed class JamendoController : ControllerBase
    {
        private readonly IJamendoService _jamendoService;

        public JamendoController(IJamendoService jamendoService)
        {
            _jamendoService = jamendoService;
        }

        [HttpGet("tracks/search")]
        public async Task<IActionResult> SearchTracks(
            [FromQuery] string query,
            [FromQuery] int limit = 20,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query is required.");
            }

            var tracks = await _jamendoService.SearchTracksAsync(
                query,
                limit,
                cancellationToken);

            return Ok(tracks);
        }

        [HttpGet("tracks/{trackId}")]
        public async Task<IActionResult> GetTrack(
            string trackId,
            CancellationToken cancellationToken)
        {
            var track = await _jamendoService.GetTrackAsync(
                trackId,
                cancellationToken);

            if (track is null)
            {
                return NotFound();
            }

            return Ok(track);
        }

        [HttpGet("albums/search")]
        public async Task<IActionResult> SearchAlbums(
            [FromQuery] string query,
            [FromQuery] int limit = 20,
            CancellationToken cancellationToken = default)
        {
            var albums = await _jamendoService.SearchAlbumsAsync(
                query,
                limit,
                cancellationToken);

            return Ok(albums);
        }

        [HttpGet("albums/{albumId}")]
        public async Task<IActionResult> GetAlbum(
            string albumId,
            CancellationToken cancellationToken)
        {
            var album = await _jamendoService.GetAlbumAsync(
                albumId,
                cancellationToken);

            if (album is null)
            {
                return NotFound();
            }

            return Ok(album);
        }

        [HttpGet("tracks/{trackId}/playback")]
        public async Task<IActionResult> GetPlayback(
            string trackId,
            CancellationToken cancellationToken)
        {
            var streamUrl = await _jamendoService.GetTrackStreamUrlAsync(
                trackId,
                cancellationToken);

            if (streamUrl is null)
            {
                return NotFound();
            }

            return Ok(new
            {
                streamUrl,
                isExternalStream = true
            });
        }
    }
}
