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
    [FromQuery] int offset = 0,
    [FromQuery] int limit = 20,
    CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query is required.");
            }

            if (offset < 0)
            {
                return BadRequest("Offset cannot be negative.");
            }

            if (limit <= 0 || limit > 200)
            {
                return BadRequest("Limit must be between 1 and 200.");
            }

            var tracks = await _jamendoService.SearchTracksAsync(
                query,
                offset,
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

        [HttpGet("authors/search")]
        public async Task<IActionResult> SearchAuthors(
            [FromQuery] string query,
            [FromQuery] int limit = 20,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query is required.");
            }

            if (!IsValidLimit(limit))
            {
                return BadRequest("Limit must be between 1 and 200.");
            }

            var authors = await _jamendoService.SearchAuthorsAsync(
                query,
                limit,
                cancellationToken);

            return Ok(authors);
        }

        [HttpGet("authors/{authorId}")]
        public async Task<IActionResult> GetAuthor(
            string authorId,
            CancellationToken cancellationToken)
        {
            var author = await _jamendoService.GetAuthorAsync(authorId, cancellationToken);

            return author is null ? NotFound() : Ok(author);
        }

        [HttpGet("authors/{authorId}/tracks")]
        public async Task<IActionResult> GetTracksByAuthor(
            string authorId,
            [FromQuery] int limit = 50,
            CancellationToken cancellationToken = default)
        {
            if (!IsValidLimit(limit))
            {
                return BadRequest("Limit must be between 1 and 200.");
            }

            var result = await _jamendoService.GetTracksByAuthorAsync(
                authorId,
                limit,
                cancellationToken);

            return result is null ? NotFound() : Ok(result);
        }

        [HttpGet("authors/{authorId}/albums")]
        public async Task<IActionResult> GetAlbumsByAuthor(
            string authorId,
            [FromQuery] int limit = 50,
            CancellationToken cancellationToken = default)
        {
            if (!IsValidLimit(limit))
            {
                return BadRequest("Limit must be between 1 and 200.");
            }

            var result = await _jamendoService.GetAlbumsByAuthorAsync(
                authorId,
                limit,
                cancellationToken);

            return result is null ? NotFound() : Ok(result);
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

        private static bool IsValidLimit(int limit) => limit is >= 1 and <= 200;
    }
}
