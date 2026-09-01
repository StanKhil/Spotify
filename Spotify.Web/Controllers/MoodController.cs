using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spotify.Application.DTOs.Mood;
using Spotify.Application.Interfaces;

namespace Spotify.Web.Controllers;

[ApiController]
[Route("api/admin/moods")]
[Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class MoodController : ControllerBase
{
    private readonly IMoodService _moodService;

    public MoodController(IMoodService moodService)
    {
        _moodService = moodService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<MoodResponse>>> GetMoods(
        CancellationToken cancellationToken)
    {
        var moods = await _moodService.GetMoodsAsync(cancellationToken);
        return Ok(moods);
    }

    [HttpPost]
    public async Task<ActionResult<MoodResponse>> CreateMood(
        [FromBody] CreateMoodRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _moodService.CreateMoodAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return ValidationProblem(ModelState);
        }

        return StatusCode(StatusCodes.Status201Created, result.Mood);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MoodResponse>> EditMood(
        Guid id,
        [FromBody] UpdateMoodRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _moodService.EditMoodAsync(id, request, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return ValidationProblem(ModelState);
        }

        return Ok(result.Mood);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMood(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _moodService.DeleteMoodAsync(id, cancellationToken);

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