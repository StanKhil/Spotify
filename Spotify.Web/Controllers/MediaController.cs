using Microsoft.AspNetCore.Mvc;
using Spotify.Application.Interfaces;

namespace Spotify.Web.Controllers;

[ApiController]
[Route("api/admin/media")]
public sealed class MediaController : ControllerBase
{
    private readonly IMediaService _mediaService;

    public MediaController(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    [HttpPost("audio")]
    [RequestSizeLimit(50 * 1024 * 1024)]
    public async Task<IActionResult> UploadAudio(IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length == 0)
        {
            return BadRequest("File is empty.");
        }

        await using var stream = file.OpenReadStream();
        var result = await _mediaService.UploadAudioAsync(stream, file.FileName, file.ContentType, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            return ValidationProblem(ModelState);
        }

        return Ok(new { itemId = result.ItemId, url = result.Url });
    }

    [HttpPost("image")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> UploadImage(IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length == 0)
        {
            return BadRequest("File is empty.");
        }

        await using var stream = file.OpenReadStream();
        var result = await _mediaService.UploadImageAsync(stream, file.FileName, file.ContentType, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            return ValidationProblem(ModelState);
        }

        return Ok(new { itemId = result.ItemId, url = result.Url });
    }
}