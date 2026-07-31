using Microsoft.AspNetCore.Mvc;
using Spotify.Application.DTOs.Playlist;
using Spotify.Application.Interfaces;

namespace Spotify.Web.Controllers;

[ApiController]
[Route("api/admin/playlists")]
public sealed class PlaylistController : ControllerBase
{
    private readonly IPlaylistService _playlistService;

    public PlaylistController(IPlaylistService playlistService)
    {
        _playlistService = playlistService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<PlaylistResponse>>> GetPlaylists(CancellationToken cancellationToken)
        => Ok(await _playlistService.GetPlaylistsAsync(cancellationToken));

    [HttpGet("{id}")]
    public async Task<ActionResult<PlaylistResponse>> GetPlaylistById(Guid id, CancellationToken cancellationToken)
    {
        var playlist = await _playlistService.GetPlaylistByIdAsync(id, cancellationToken);
        return playlist is null ? NotFound() : Ok(playlist);
    }

    [HttpPost]
    public async Task<ActionResult<PlaylistResponse>> CreatePlaylist(
        [FromBody] CreatePlaylistRequest request, CancellationToken cancellationToken)
    {
        var result = await _playlistService.CreatePlaylistAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            return ValidationProblem(ModelState);
        }

        return StatusCode(StatusCodes.Status201Created, result.Playlist);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PlaylistResponse>> EditPlaylist(
        Guid id, [FromBody] UpdatePlaylistRequest request, CancellationToken cancellationToken)
    {
        var result = await _playlistService.EditPlaylistAsync(id, request, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            return ValidationProblem(ModelState);
        }

        return Ok(result.Playlist);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlaylist(Guid id, CancellationToken cancellationToken)
    {
        var result = await _playlistService.DeletePlaylistAsync(id, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            return ValidationProblem(ModelState);
        }

        return NoContent();
    }
}