using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Track;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.Content;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class TrackService : ITrackService
{
    private readonly ApplicationContext _context;
    private readonly IFileStorageService _fileStorageService;

    public TrackService(ApplicationContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public async Task<IReadOnlyCollection<TrackResponse>> GetTracksAsync(
        CancellationToken cancellationToken = default)
    {
        var tracks = await _context.Tracks
            .Where(x => x.DeletedAt == null)
            .Include(x => x.TrackTags)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return tracks.Select(MapToResponse).ToList();
    }

    public async Task<TrackResponse?> GetTrackByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var track = await _context.Tracks
            .Include(x => x.TrackTags)
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null, cancellationToken);

        return track is null ? null : MapToResponse(track);
    }

    public async Task<CreateTrackResult> CreateTrackAsync(
        CreateTrackRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _context.Albums.AnyAsync(x => x.Id == request.AlbumId, cancellationToken))
        {
            return CreateTrackResult.Failure("The specified album was not found");
        }

        if (!await _context.AudioItems.AnyAsync(x => x.Id == request.AudioItemId, cancellationToken))
        {
            return CreateTrackResult.Failure("The specified audio item was not found");
        }

        if (request.MoodId is Guid moodId &&
            !await _context.Moods.AnyAsync(x => x.Id == moodId, cancellationToken))
        {
            return CreateTrackResult.Failure("The specified mood was not found");
        }

        if (request.GenreId is string genreId &&
            !await _context.Genres.AnyAsync(x => x.Id == genreId, cancellationToken))
        {
            return CreateTrackResult.Failure("The specified genre was not found");
        }

        if (request.ImageItemId is Guid imageId &&
            !await _context.ImageItems.AnyAsync(x => x.Id == imageId, cancellationToken))
        {
            return CreateTrackResult.Failure("The specified image item was not found");
        }

        var existingTagIds = await _context.Tags
            .Where(x => request.TagIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var missingTags = request.TagIds.Except(existingTagIds).ToList();
        if (missingTags.Count > 0)
        {
            return CreateTrackResult.Failure($"The following tags were not found: {string.Join(", ", missingTags)}");
        }

        var track = new Track
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            DurationSeconds = 0,
            AlbumId = request.AlbumId,
            MoodId = request.MoodId,
            GenreId = request.GenreId,
            AudioItemId = request.AudioItemId,
            ImageItemId = request.ImageItemId,
            IsAdult = request.IsAdult,
            IsDraft = request.IsDraft,
            PlaysNumber = 0,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var tagId in existingTagIds)
        {
            track.TrackTags.Add(new TrackTag { TrackId = track.Id, TagId = tagId });
        }

        _context.Tracks.Add(track);
        await _context.SaveChangesAsync(cancellationToken);

        return CreateTrackResult.Success(MapToResponse(track));
    }

    public async Task<UpdateTrackResult> EditTrackAsync(
        Guid id, UpdateTrackRequest request, CancellationToken cancellationToken = default)
    {
        var track = await _context.Tracks
            .Include(x => x.TrackTags)
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null, cancellationToken);

        if (track is null)
        {
            return UpdateTrackResult.Failure("Track was not found");
        }

        if (!await _context.Albums.AnyAsync(x => x.Id == request.AlbumId, cancellationToken))
        {
            return UpdateTrackResult.Failure("The specified album was not found");
        }

        if (request.MoodId is Guid moodId &&
            !await _context.Moods.AnyAsync(x => x.Id == moodId, cancellationToken))
        {
            return UpdateTrackResult.Failure("The specified mood was not found");
        }

        if (request.GenreId is string genreId &&
            !await _context.Genres.AnyAsync(x => x.Id == genreId, cancellationToken))
        {
            return UpdateTrackResult.Failure("The specified genre was not found");
        }

        var existingTagIds = await _context.Tags
            .Where(x => request.TagIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var missingTags = request.TagIds.Except(existingTagIds).ToList();
        if (missingTags.Count > 0)
        {
            return UpdateTrackResult.Failure($"The following tags were not found: {string.Join(", ", missingTags)}");
        }

        track.Name = request.Name.Trim();
        track.Description = request.Description?.Trim();
        track.AlbumId = request.AlbumId;
        track.MoodId = request.MoodId;
        track.GenreId = request.GenreId;
        track.ImageItemId = request.ImageItemId;
        track.IsAdult = request.IsAdult;
        track.IsDraft = request.IsDraft;

        var tagsToRemove = track.TrackTags.Where(x => !existingTagIds.Contains(x.TagId)).ToList();
        foreach (var tagToRemove in tagsToRemove)
        {
            track.TrackTags.Remove(tagToRemove);
        }

        var currentTagIds = track.TrackTags.Select(x => x.TagId).ToHashSet();
        foreach (var tagId in existingTagIds.Where(tagId => !currentTagIds.Contains(tagId)))
        {
            track.TrackTags.Add(new TrackTag { TrackId = track.Id, TagId = tagId });
        }

        await _context.SaveChangesAsync(cancellationToken);

        return UpdateTrackResult.Success(MapToResponse(track));
    }

    public async Task<DeleteTrackResult> DeleteTrackAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var track = await _context.Tracks
            .Include(x => x.AudioItem)
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null, cancellationToken);

        if (track is null)
        {
            return DeleteTrackResult.Failure("Track was not found");
        }

        if (track.AudioItem is not null)
        {
            if (!string.IsNullOrWhiteSpace(track.AudioItem.StorageKey))
            {
                await _fileStorageService.DeleteAsync(track.AudioItem.StorageKey, cancellationToken);
            }

            _context.AudioItems.Remove(track.AudioItem);
        }

        if (track.ImageItemId is Guid imageItemId)
        {
            await DeleteImageItemIfOrphanedAsync(imageItemId, new HashSet<Guid> { track.Id }, cancellationToken);
        }

        track.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return DeleteTrackResult.Success();
    }

    public async Task<BatchDeleteTracksResult> BatchDeleteTracksAsync(
        BatchDeleteTracksRequest request, CancellationToken cancellationToken = default)
    {
        var tracks = await _context.Tracks
            .Include(x => x.AudioItem)
            .Where(x => request.TrackIds.Contains(x.Id) && x.DeletedAt == null)
            .ToListAsync(cancellationToken);

        var batchTrackIds = tracks.Select(x => x.Id).ToHashSet();
        var now = DateTime.UtcNow;

        foreach (var track in tracks)
        {
            if (track.AudioItem is not null)
            {
                if (!string.IsNullOrWhiteSpace(track.AudioItem.StorageKey))
                {
                    await _fileStorageService.DeleteAsync(track.AudioItem.StorageKey, cancellationToken);
                }

                _context.AudioItems.Remove(track.AudioItem);
            }

            if (track.ImageItemId is Guid imageItemId)
            {
                await DeleteImageItemIfOrphanedAsync(imageItemId, batchTrackIds, cancellationToken);
            }

            track.DeletedAt = now;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return BatchDeleteTracksResult.Success(tracks.Count);
    }

    private async Task<bool> IsImageItemInUseAsync(
        Guid imageItemId,
        ISet<Guid> excludedTrackIds,
        CancellationToken cancellationToken)
    {
        var usedByTrack = await _context.Tracks.AnyAsync(
            x => x.ImageItemId == imageItemId &&
                 x.DeletedAt == null &&
                 !excludedTrackIds.Contains(x.Id),
            cancellationToken);

        if (usedByTrack)
        {
            return true;
        }

        var usedByAlbum = await _context.Albums.AnyAsync(
            x => x.ImageItemId == imageItemId && x.DeletedAt == null,
            cancellationToken);

        if (usedByAlbum)
        {
            return true;
        }

        var usedByEpisode = await _context.Episodes.AnyAsync(
            x => x.ImageItemId == imageItemId && x.DeletedAt == null,
            cancellationToken);

        if (usedByEpisode)
        {
            return true;
        }

        var usedByAudiobook = await _context.Audiobooks.AnyAsync(
            x => x.ImageItemId == imageItemId && x.DeletedAt == null,
            cancellationToken);

        if (usedByAudiobook)
        {
            return true;
        }

        var usedByMood = await _context.Moods.AnyAsync(
            x => x.MoodImageId == imageItemId,
            cancellationToken);

        return usedByMood;
    }

    private async Task DeleteImageItemIfOrphanedAsync(
        Guid imageItemId,
        ISet<Guid> excludedTrackIds,
        CancellationToken cancellationToken)
    {
        var isInUse = await IsImageItemInUseAsync(imageItemId, excludedTrackIds, cancellationToken);

        if (isInUse)
        {
            return;
        }

        var imageItem = await _context.ImageItems
            .FirstOrDefaultAsync(x => x.Id == imageItemId, cancellationToken);

        if (imageItem is null)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(imageItem.ImageList))
        {
            await _fileStorageService.DeleteAsync(imageItem.ImageList, cancellationToken);
        }

        _context.ImageItems.Remove(imageItem);
    }

    private static TrackResponse MapToResponse(Track track) => new(
        track.Id, track.Name, track.Description, track.DurationSeconds,
        track.AlbumId ?? Guid.Empty, track.MoodId, track.GenreId, track.PlaysNumber,
        track.IsAdult, track.IsDraft, track.AudioItemId, track.ImageItemId,
        track.TrackTags.Select(x => x.TagId).ToList(), track.CreatedAt);
}