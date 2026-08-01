using Microsoft.AspNetCore.Mvc;
using Spotify.Application.DTOs.Album;
using Spotify.Application.Interfaces;

namespace Spotify.Web.Controllers;

[ApiController]
[Route("api/admin/albums")]
public sealed class AlbumController : ControllerBase
{
    private readonly IAlbumService _albumService;

    public AlbumController(IAlbumService albumService)
    {
        _albumService = albumService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<AlbumResponse>>> GetAlbums(
        CancellationToken cancellationToken)
    {
        var albums = await _albumService.GetAlbumsAsync(cancellationToken);
        return Ok(albums);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AlbumResponse>> GetAlbumById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var album = await _albumService.GetAlbumByIdAsync(id, cancellationToken);
        return album is null ? NotFound() : Ok(album);
    }

    [HttpPost]
    public async Task<ActionResult<AlbumResponse>> CreateAlbum(
        [FromBody] CreateAlbumRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _albumService.CreateAlbumAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);

            return ValidationProblem(ModelState);
        }

        return StatusCode(StatusCodes.Status201Created, result.Album);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AlbumResponse>> EditAlbum(
        Guid id,
        [FromBody] UpdateAlbumRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _albumService.EditAlbumAsync(id, request, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);

            return ValidationProblem(ModelState);
        }

        return Ok(result.Album);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAlbum(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _albumService.DeleteAlbumAsync(id, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);

            return ValidationProblem(ModelState);
        }

        return NoContent();
    }
}