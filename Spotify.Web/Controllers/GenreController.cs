using Microsoft.AspNetCore.Mvc;
using Spotify.Application.DTOs.Genre;
using Spotify.Application.Interfaces;

namespace Spotify.Web.Controllers;

[ApiController]
[Route("api/admin/genres")]
public sealed class GenreController : ControllerBase
{
    private readonly IGenreService _genreService;

    public GenreController(IGenreService genreService)
    {
        _genreService = genreService;
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<GenreResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<GenreResponse>>> GetGenres(
        CancellationToken cancellationToken)
    {
        var genres = await _genreService.GetGenresAsync(cancellationToken);
        return Ok(genres);
    }

    [HttpPost]
    [ProducesResponseType<GenreResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GenreResponse>> CreateGenre(
        [FromBody] CreateGenreRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _genreService.CreateGenreAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return ValidationProblem(ModelState);
        }

        return StatusCode(StatusCodes.Status201Created, result.Genre);
    }

    [HttpPut("{id}")]
    [ProducesResponseType<GenreResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GenreResponse>> EditGenre(
        string id,
        [FromBody] UpdateGenreRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _genreService.EditGenreAsync(id, request, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return ValidationProblem(ModelState);
        }

        return Ok(result.Genre);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteGenre(
        string id,
        CancellationToken cancellationToken)
    {
        var result = await _genreService.DeleteGenreAsync(id, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return ValidationProblem(ModelState);
        }

        return NoContent();
    }
}