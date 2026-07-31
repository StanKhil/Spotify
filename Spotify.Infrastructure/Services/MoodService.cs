using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Mood;
using Spotify.Application.Interfaces;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class MoodService : IMoodService
{
    private readonly ApplicationContext _context;

    public MoodService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<MoodResponse>> GetMoodsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Moods
            .OrderBy(x => x.Name)
            .Select(x => new MoodResponse(x.Id, x.Name, x.MoodImageId))
            .ToListAsync(cancellationToken);
    }

    public async Task<CreateMoodResult> CreateMoodAsync(
        CreateMoodRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.MoodImageId is Guid imageId &&
            !await _context.ImageItems.AnyAsync(x => x.Id == imageId, cancellationToken))
        {
            return CreateMoodResult.Failure("The specified mood image was not found");
        }

        var mood = new Domain.Entities.Content.Mood
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            MoodImageId = request.MoodImageId
        };

        _context.Moods.Add(mood);
        await _context.SaveChangesAsync(cancellationToken);

        return CreateMoodResult.Success(new MoodResponse(mood.Id, mood.Name, mood.MoodImageId));
    }

    public async Task<UpdateMoodResult> EditMoodAsync(
        Guid id,
        UpdateMoodRequest request,
        CancellationToken cancellationToken = default)
    {
        var mood = await _context.Moods.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (mood is null)
        {
            return UpdateMoodResult.Failure("Mood was not found");
        }

        if (request.MoodImageId is Guid imageId &&
            !await _context.ImageItems.AnyAsync(x => x.Id == imageId, cancellationToken))
        {
            return UpdateMoodResult.Failure("The specified mood image was not found");
        }

        mood.Name = request.Name.Trim();
        mood.MoodImageId = request.MoodImageId;

        await _context.SaveChangesAsync(cancellationToken);

        return UpdateMoodResult.Success(new MoodResponse(mood.Id, mood.Name, mood.MoodImageId));
    }

    public async Task<DeleteMoodResult> DeleteMoodAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var mood = await _context.Moods.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (mood is null)
        {
            return DeleteMoodResult.Failure("Mood was not found");
        }

        var isInUse = await _context.Tracks.AnyAsync(x => x.MoodId == id, cancellationToken);

        if (isInUse)
        {
            return DeleteMoodResult.Failure("Cannot delete a mood that is used by existing tracks");
        }

        _context.Moods.Remove(mood);
        await _context.SaveChangesAsync(cancellationToken);

        return DeleteMoodResult.Success();
    }
}