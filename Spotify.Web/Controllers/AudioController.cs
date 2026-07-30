using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spotify.Infrastructure.Persistance.Context;
using Spotify.Infrastructure.Playback;

namespace Spotify.Web.Controllers;

[ApiController]
[Route("api/audio")]
public sealed class AudioController : ControllerBase
{
    private readonly ApplicationContext _context;
    private readonly ILocalAudioStorageService _localAudioStorageService;
    private readonly ILocalPlaybackUrlService _localPlaybackUrlService;

    public AudioController(
        ApplicationContext context,
        ILocalAudioStorageService localAudioStorageService,
        ILocalPlaybackUrlService localPlaybackUrlService)
    {
        _context = context;
        _localAudioStorageService = localAudioStorageService;
        _localPlaybackUrlService = localPlaybackUrlService;
    }

    [HttpGet("{audioItemId:guid}/stream")]
    public async Task<IActionResult> Stream(
        Guid audioItemId,
        [FromQuery] long expiresAt,
        [FromQuery] string signature,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(signature))
        {
            return Unauthorized();
        }

        if (!_localPlaybackUrlService.IsValid(audioItemId, expiresAt, signature))
        {
            return Unauthorized();
        }

        var audioItem = await _context.AudioItems
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == audioItemId, cancellationToken);

        if (audioItem is null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(audioItem.StorageKey))
        {
            return NotFound();
        }

        var filePath = _localAudioStorageService.GetSafeFilePath(audioItem.StorageKey);

        if (filePath is null || !System.IO.File.Exists(filePath))
        {
            return NotFound();
        }

        return PhysicalFile(
            filePath,
            "audio/mpeg",
            enableRangeProcessing: true);
    }
}