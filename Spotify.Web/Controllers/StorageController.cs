using Microsoft.AspNetCore.Mvc;
using Spotify.Application.Interfaces;

namespace Spotify.Web.Controllers;

[ApiController]
[Route("api/storage")]
public sealed class StorageController : ControllerBase
{
    private readonly IStorageService _storageService;

    public StorageController(IStorageService storageService)
    {
        _storageService = storageService;
    }

    [HttpGet("images/{imageId:guid}")]
    [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
    [Produces("image/jpeg", "image/png", "image/webp")]
    public async Task<IActionResult> GetImage(
        Guid imageId,
        CancellationToken cancellationToken)
    {
        var image = await _storageService.GetImageAsync(imageId, cancellationToken);

        return image is null
            ? NotFound()
            : File(image.Content, image.ContentType);
    }
}
