using Microsoft.AspNetCore.Mvc;
using Spotify.Application.DTOs.Author;
using Spotify.Application.Interfaces;

namespace Spotify.Web.Controllers;

[ApiController]
[Route("api/admin/authors")]
public sealed class AuthorController : ControllerBase
{
    private readonly IAuthorService _authorService;

    public AuthorController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<AuthorResponse>>> GetAuthors(CancellationToken cancellationToken)
        => Ok(await _authorService.GetAuthorsAsync(cancellationToken));

    [HttpGet("{id}")]
    public async Task<ActionResult<AuthorResponse>> GetAuthorById(Guid id, CancellationToken cancellationToken)
    {
        var author = await _authorService.GetAuthorByIdAsync(id, cancellationToken);
        return author is null ? NotFound() : Ok(author);
    }

    [HttpPost]
    public async Task<ActionResult<AuthorResponse>> CreateAuthor(
        [FromBody] CreateAuthorRequest request, CancellationToken cancellationToken)
    {
        var result = await _authorService.CreateAuthorAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            return ValidationProblem(ModelState);
        }

        return StatusCode(StatusCodes.Status201Created, result.Author);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuthor(Guid id, CancellationToken cancellationToken)
    {
        var result = await _authorService.DeleteAuthorAsync(id, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            return ValidationProblem(ModelState);
        }

        return NoContent();
    }
}