using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spotify.Application.DTOs.AuthorUpload;
using Spotify.Application.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Spotify.Web.Controllers;

[ApiController]
[Route("api/author/upload")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class AuthorUploadController : ControllerBase
{
    private const long MaxCoverBytes = 10 * 1024 * 1024;
    private const long MaxCoverRequestBytes = 11 * 1024 * 1024;
    private const long MaxAudioRequestBytes = 55 * 1024 * 1024;

    private readonly IAuthorUploadService _authorUploadService;
    private readonly IGenreService _genreService;
    private readonly IMoodService _moodService;

    public AuthorUploadController(
        IAuthorUploadService authorUploadService,
        IGenreService genreService,
        IMoodService moodService)
    {
        _authorUploadService = authorUploadService;
        _genreService = genreService;
        _moodService = moodService;
    }

    [HttpGet("genres")]
    public async Task<IActionResult> GetGenres(CancellationToken cancellationToken)
    {
        return Ok(await _genreService.GetGenresAsync(cancellationToken));
    }

    [HttpGet("moods")]
    public async Task<IActionResult> GetMoods(CancellationToken cancellationToken)
    {
        return Ok(await _moodService.GetMoodsAsync(cancellationToken));
    }

    [HttpGet("albums")]
    public async Task<IActionResult> GetMyAlbums(CancellationToken cancellationToken)
    {
        var result = await _authorUploadService.GetMyAlbumsAsync(cancellationToken);
        return result.Succeeded ? Ok(result.Albums) : Fail(result);
    }

    [HttpGet("tracks")]
    public async Task<IActionResult> GetMyTracks(CancellationToken cancellationToken)
    {
        var result = await _authorUploadService.GetMyTracksAsync(cancellationToken);
        return result.Succeeded ? Ok(result.Tracks) : Fail(result);
    }

    [HttpGet("albums/{albumId:guid}/tracks")]
    public async Task<IActionResult> GetAlbumTracks(Guid albumId, CancellationToken cancellationToken)
    {
        var result = await _authorUploadService.GetAlbumTracksAsync(albumId, cancellationToken);
        return result.Succeeded ? Ok(result.Tracks) : Fail(result);
    }

    [HttpPost("albums")]
    [RequestSizeLimit(MaxCoverRequestBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxCoverRequestBytes)]
    public async Task<IActionResult> CreateAlbum(
        [FromForm] AlbumUploadForm form,
        CancellationToken cancellationToken)
    {
        if (form.Cover is null || form.Cover.Length == 0)
        {
            ModelState.AddModelError(nameof(form.Cover), "Cover is required.");
            return ValidationProblem(ModelState);
        }

        if (form.Cover.Length > MaxCoverBytes)
        {
            ModelState.AddModelError(nameof(form.Cover), $"Cover is too large. Max size is {MaxCoverBytes / 1024 / 1024} MB.");
            return ValidationProblem(ModelState);
        }

        await using var stream = form.Cover.OpenReadStream();

        var result = await _authorUploadService.CreateAlbumAsync(
            new AuthorAlbumUploadRequest(
                form.Name,
                form.Description,
                NormalizeId(form.GenreId),
                new UploadedFile(stream, form.Cover.FileName, form.Cover.ContentType)),
            cancellationToken);

        return result.Succeeded ? StatusCode(StatusCodes.Status201Created, result.Album) : Fail(result);
    }

    [HttpPut("albums/{albumId:guid}")]
    [RequestSizeLimit(MaxCoverRequestBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxCoverRequestBytes)]
    public async Task<IActionResult> UpdateAlbum(
        Guid albumId,
        [FromForm] AlbumUploadForm form,
        CancellationToken cancellationToken)
    {
        if (form.Cover is { Length: > MaxCoverBytes })
        {
            ModelState.AddModelError(nameof(form.Cover), $"Cover is too large. Max size is {MaxCoverBytes / 1024 / 1024} MB.");
            return ValidationProblem(ModelState);
        }

        await using var stream = form.Cover is { Length: > 0 } ? form.Cover.OpenReadStream() : null;

        var cover = stream is null
            ? null
            : new UploadedFile(stream, form.Cover!.FileName, form.Cover.ContentType);

        var result = await _authorUploadService.UpdateAlbumAsync(
            new AuthorAlbumUpdateRequest(
                albumId,
                form.Name,
                form.Description,
                NormalizeId(form.GenreId),
                cover),
            cancellationToken);

        return result.Succeeded ? Ok(result.Album) : Fail(result);
    }

    [HttpDelete("albums/{albumId:guid}")]
    public async Task<IActionResult> DeleteAlbum(Guid albumId, CancellationToken cancellationToken)
    {
        var result = await _authorUploadService.DeleteAlbumAsync(albumId, cancellationToken);
        return result.Succeeded ? NoContent() : Fail(result);
    }

    [HttpPost("albums/{albumId:guid}/publish")]
    public async Task<IActionResult> PublishAlbum(Guid albumId, CancellationToken cancellationToken)
    {
        var result = await _authorUploadService.PublishAlbumAsync(albumId, cancellationToken);
        return result.Succeeded ? Ok(result.Album) : Fail(result);
    }

    [HttpPost("albums/{albumId:guid}/tracks")]
    [RequestSizeLimit(MaxAudioRequestBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxAudioRequestBytes)]
    public async Task<IActionResult> AddTrack(
        Guid albumId,
        [FromForm] TrackUploadForm form,
        CancellationToken cancellationToken)
    {
        if (form.Audio is null || form.Audio.Length == 0)
        {
            ModelState.AddModelError(nameof(form.Audio), "Audio file is required.");
            return ValidationProblem(ModelState);
        }

        await using var stream = form.Audio.OpenReadStream();

        var result = await _authorUploadService.AddTrackAsync(
            new AuthorTrackUploadRequest(
                albumId,
                form.Name,
                form.Description,
                NormalizeId(form.GenreId),
                form.MoodId,
                form.IsAdult,
                form.IsDraft,
                new UploadedFile(stream, form.Audio.FileName, form.Audio.ContentType)),
            cancellationToken);

        return result.Succeeded ? StatusCode(StatusCodes.Status201Created, result.Track) : Fail(result);
    }

    [HttpPut("tracks/{trackId:guid}")]
    public async Task<IActionResult> UpdateTrack(
        Guid trackId,
        [FromBody] TrackUpdateBody body,
        CancellationToken cancellationToken)
    {
        var result = await _authorUploadService.UpdateTrackAsync(
            new AuthorTrackUpdateRequest(
                trackId,
                body.Name,
                body.Description,
                NormalizeId(body.GenreId),
                body.MoodId,
                body.IsAdult,
                body.IsDraft),
            cancellationToken);

        return result.Succeeded ? Ok(result.Track) : Fail(result);
    }

    [HttpDelete("tracks/{trackId:guid}")]
    public async Task<IActionResult> DeleteTrack(Guid trackId, CancellationToken cancellationToken)
    {
        var result = await _authorUploadService.DeleteTrackAsync(trackId, cancellationToken);
        return result.Succeeded ? NoContent() : Fail(result);
    }

    private static string? NormalizeId(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private IActionResult Fail(IAuthorUploadResult result)
    {
        switch (result.FailureType)
        {
            case AuthorUploadFailure.Forbidden:
                return Problem(detail: string.Join("; ", result.Errors), statusCode: StatusCodes.Status403Forbidden);

            case AuthorUploadFailure.NotFound:
                return Problem(detail: string.Join("; ", result.Errors), statusCode: StatusCodes.Status404NotFound);

            default:
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return ValidationProblem(ModelState);
        }
    }
}

public sealed class AlbumUploadForm
{
    [Required]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; init; }

    public string? GenreId { get; init; }

    public IFormFile? Cover { get; init; }
}

public sealed class TrackUploadForm
{
    [Required]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; init; }

    public string? GenreId { get; init; }

    public Guid? MoodId { get; init; }

    public bool IsAdult { get; init; }

    public bool IsDraft { get; init; } = true;

    public IFormFile? Audio { get; init; }
}

public sealed class TrackUpdateBody
{
    [Required]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; init; }

    public string? GenreId { get; init; }

    public Guid? MoodId { get; init; }

    public bool IsAdult { get; init; }

    public bool IsDraft { get; init; }
}