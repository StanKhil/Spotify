using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Podcast;
using Spotify.Application.Interfaces;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class PodcastService : IPodcastService
{
    private readonly ApplicationContext _context;

    public PodcastService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<PodcastResponse>> GetPodcastsAsync(
        CancellationToken cancellationToken = default)
    {
        var podcasts = await _context.Podcasts
            .Include(x => x.Authors)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return podcasts.Select(MapToResponse).ToList();
    }

    public async Task<PodcastResponse?> GetPodcastByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var podcast = await _context.Podcasts
            .Include(x => x.Authors)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return podcast is null ? null : MapToResponse(podcast);
    }

    public async Task<CreatePodcastResult> CreatePodcastAsync(
        CreatePodcastRequest request, CancellationToken cancellationToken = default)
    {
        var existingAuthorIds = await _context.Authors
            .Where(x => request.AuthorIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var missingAuthors = request.AuthorIds.Except(existingAuthorIds).ToList();
        if (missingAuthors.Count > 0)
        {
            return CreatePodcastResult.Failure($"The following authors were not found: {string.Join(", ", missingAuthors)}");
        }

        var podcast = new Domain.Entities.Content.Podcast
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description.Trim()
        };

        foreach (var authorId in existingAuthorIds)
        {
            podcast.Authors.Add(new Domain.Entities.Content.PodcastAuthor
            {
                PodcastId = podcast.Id,
                AuthorId = authorId
            });
        }

        _context.Podcasts.Add(podcast);
        await _context.SaveChangesAsync(cancellationToken);

        return CreatePodcastResult.Success(MapToResponse(podcast));
    }

    public async Task<UpdatePodcastResult> EditPodcastAsync(
        Guid id, UpdatePodcastRequest request, CancellationToken cancellationToken = default)
    {
        var podcast = await _context.Podcasts
            .Include(x => x.Authors)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (podcast is null)
        {
            return UpdatePodcastResult.Failure("Podcast was not found.");
        }

        var existingAuthorIds = await _context.Authors
            .Where(x => request.AuthorIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var missingAuthors = request.AuthorIds.Except(existingAuthorIds).ToList();
        if (missingAuthors.Count > 0)
        {
            return UpdatePodcastResult.Failure($"The following authors were not found: {string.Join(", ", missingAuthors)}");
        }

        podcast.Name = request.Name.Trim();
        podcast.Description = request.Description.Trim();

        var authorsToRemove = podcast.Authors.Where(x => !existingAuthorIds.Contains(x.AuthorId)).ToList();
        foreach (var authorToRemove in authorsToRemove)
        {
            podcast.Authors.Remove(authorToRemove);
        }

        var currentAuthorIds = podcast.Authors.Select(x => x.AuthorId).ToHashSet();
        foreach (var authorId in existingAuthorIds.Where(authorId => !currentAuthorIds.Contains(authorId)))
        {
            podcast.Authors.Add(new Domain.Entities.Content.PodcastAuthor
            {
                PodcastId = podcast.Id,
                AuthorId = authorId
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        var episodesCount = await _context.Episodes.CountAsync(x => x.PodcastId == id, cancellationToken);

        return UpdatePodcastResult.Success(new PodcastResponse(
    podcast.Id, podcast.Name, podcast.Description, episodesCount,
    podcast.Authors.Select(x => x.AuthorId).ToList(),
    podcast.CreatedAt));
    }

    public async Task<DeletePodcastResult> DeletePodcastAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var podcast = await _context.Podcasts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (podcast is null)
        {
            return DeletePodcastResult.Failure("Podcast was not found.");
        }

        var hasEpisodes = await _context.Episodes.AnyAsync(x => x.PodcastId == id, cancellationToken);

        if (hasEpisodes)
        {
            return DeletePodcastResult.Failure("Cannot delete a podcast that still has episodes.");
        }

        _context.Podcasts.Remove(podcast);
        await _context.SaveChangesAsync(cancellationToken);

        return DeletePodcastResult.Success();
    }

    private static PodcastResponse MapToResponse(Domain.Entities.Content.Podcast podcast) => new(
    podcast.Id, podcast.Name, podcast.Description,
    podcast.Episodes.Count,
    podcast.Authors.Select(x => x.AuthorId).ToList(),
    podcast.CreatedAt);
}