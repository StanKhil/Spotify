using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Episode;
using Spotify.Application.Interfaces;
using Spotify.Infrastructure.Persistance.Context;
using Spotify.Domain.Entities;

namespace Spotify.Infrastructure.Services;

public sealed class EpisodeService : IEpisodeService
{
    private readonly ApplicationContext _context;

    public EpisodeService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<EpisodeResponse>> GetEpisodesAsync(
        Guid? podcastId, Guid? audiobookId, CancellationToken cancellationToken = default)
    {
        var query = _context.Episodes.Where(x => x.DeletedAt == null);

        if (podcastId is Guid pid)
        {
            query = query.Where(x => x.PodcastId == pid);
        }

        if (audiobookId is Guid aid)
        {
            query = query.Where(x => x.AudiobookId == aid);
        }

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new EpisodeResponse(
                x.Id, x.Name, x.Description, x.DurationSeconds,
                x.PodcastId, x.AudiobookId, x.AudioItemId, x.ImageItemId, x.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<EpisodeResponse?> GetEpisodeByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Episodes
            .Where(x => x.Id == id && x.DeletedAt == null)
            .Select(x => new EpisodeResponse(
                x.Id, x.Name, x.Description, x.DurationSeconds,
                x.PodcastId, x.AudiobookId, x.AudioItemId, x.ImageItemId, x.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CreateEpisodeResult> CreateEpisodeAsync(
        CreateEpisodeRequest request, CancellationToken cancellationToken = default)
    {
        if (request.PodcastId is null && request.AudiobookId is null)
        {
            return CreateEpisodeResult.Failure("Either PodcastId or AudiobookId must be specified.");
        }

        if (request.PodcastId is not null && request.AudiobookId is not null)
        {
            return CreateEpisodeResult.Failure("Only one of PodcastId or AudiobookId can be specified.");
        }

        if (request.PodcastId is Guid podcastId &&
            !await _context.Podcasts.AnyAsync(x => x.Id == podcastId, cancellationToken))
        {
            return CreateEpisodeResult.Failure("The specified podcast was not found.");
        }

        if (request.AudiobookId is Guid audiobookId &&
            !await _context.Audiobooks.AnyAsync(x => x.Id == audiobookId, cancellationToken))
        {
            return CreateEpisodeResult.Failure("The specified audiobook was not found.");
        }

        if (!await _context.AudioItems.AnyAsync(x => x.Id == request.AudioItemId, cancellationToken))
        {
            return CreateEpisodeResult.Failure("The specified audio item was not found.");
        }

        if (request.ImageItemId is Guid imageId &&
            !await _context.ImageItems.AnyAsync(x => x.Id == imageId, cancellationToken))
        {
            return CreateEpisodeResult.Failure("The specified image item was not found.");
        }

        var episode = new Episode
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            DurationSeconds = 0,
            PodcastId = request.PodcastId,
            AudiobookId = request.AudiobookId,
            AudioItemId = request.AudioItemId,
            ImageItemId = request.ImageItemId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Episodes.Add(episode);
        await _context.SaveChangesAsync(cancellationToken);

        return CreateEpisodeResult.Success(new EpisodeResponse(
            episode.Id, episode.Name, episode.Description, episode.DurationSeconds,
            episode.PodcastId, episode.AudiobookId, episode.AudioItemId, episode.ImageItemId, episode.CreatedAt));
    }

    public async Task<UpdateEpisodeResult> EditEpisodeAsync(
        Guid id, UpdateEpisodeRequest request, CancellationToken cancellationToken = default)
    {
        var episode = await _context.Episodes
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null, cancellationToken);

        if (episode is null)
        {
            return UpdateEpisodeResult.Failure("Episode was not found.");
        }

        if (request.ImageItemId is Guid imageId &&
            !await _context.ImageItems.AnyAsync(x => x.Id == imageId, cancellationToken))
        {
            return UpdateEpisodeResult.Failure("The specified image item was not found.");
        }

        episode.Name = request.Name.Trim();
        episode.Description = request.Description?.Trim();
        episode.ImageItemId = request.ImageItemId;

        await _context.SaveChangesAsync(cancellationToken);

        return UpdateEpisodeResult.Success(new EpisodeResponse(
            episode.Id, episode.Name, episode.Description, episode.DurationSeconds,
            episode.PodcastId, episode.AudiobookId, episode.AudioItemId, episode.ImageItemId, episode.CreatedAt));
    }

    public async Task<DeleteEpisodeResult> DeleteEpisodeAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var episode = await _context.Episodes
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null, cancellationToken);

        if (episode is null)
        {
            return DeleteEpisodeResult.Failure("Episode was not found.");
        }

        episode.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return DeleteEpisodeResult.Success();
    }
}