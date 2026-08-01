using Microsoft.AspNetCore.Mvc;
using Spotify.Application.DTOs.Audiobook;
using Spotify.Application.Interfaces;

namespace Spotify.Web.Controllers;

[ApiController]
[Route("api/admin/audiobooks")]
public sealed class AudiobookController : ControllerBase
{
    private readonly IAudiobookService _audiobookService;

    public AudiobookController(IAudiobookService audiobookService)
    {
        _audiobookService = audiobookService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<AudiobookResponse>>> GetAudiobooks(CancellationToken cancellationToken)
        => Ok(await _audiobookService.GetAudiobooksAsync(cancellationToken));

    [HttpGet("{id}")]
    public async Task<ActionResult<AudiobookResponse>> GetAudiobookById(Guid id, CancellationToken cancellationToken)
    {
        var audiobook = await _audiobookService.GetAudiobookByIdAsync(id, cancellationToken);
        return audiobook is null ? NotFound() : Ok(audiobook);
    }

    [HttpPost]
    public async Task<ActionResult<AudiobookResponse>> CreateAudiobook(
        [FromBody] CreateAudiobookRequest request, CancellationToken cancellationToken)
    {
        var result = await _audiobookService.CreateAudiobookAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            return ValidationProblem(ModelState);
        }

        return StatusCode(StatusCodes.Status201Created, result.Audiobook);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AudiobookResponse>> EditAudiobook(
        Guid id, [FromBody] UpdateAudiobookRequest request, CancellationToken cancellationToken)
    {
        var result = await _audiobookService.EditAudiobookAsync(id, request, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            return ValidationProblem(ModelState);
        }

        return Ok(result.Audiobook);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAudiobook(Guid id, CancellationToken cancellationToken)
    {
        var result = await _audiobookService.DeleteAudiobookAsync(id, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            return ValidationProblem(ModelState);
        }

        return NoContent();
    }
}