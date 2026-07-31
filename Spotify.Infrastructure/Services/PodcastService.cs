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
        return await _context.Podcasts
            .OrderBy(x => x.Name)
            .Select(x => new PodcastResponse(x.Id, x.Name, x.Description, x.Episodes.Count))
            .ToListAsync(cancellationToken);
    }

    public async Task<PodcastResponse?> GetPodcastByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Podcasts
            .Where(x => x.Id == id)
            .Select(x => new PodcastResponse(x.Id, x.Name, x.Description, x.Episodes.Count))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CreatePodcastResult> CreatePodcastAsync(
        CreatePodcastRequest request, CancellationToken cancellationToken = default)
    {
        var podcast = new Domain.Entities.Content.Podcast
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description.Trim()
        };

        _context.Podcasts.Add(podcast);
        await _context.SaveChangesAsync(cancellationToken);

        return CreatePodcastResult.Success(new PodcastResponse(podcast.Id, podcast.Name, podcast.Description, 0));
    }

    public async Task<UpdatePodcastResult> EditPodcastAsync(
        Guid id, UpdatePodcastRequest request, CancellationToken cancellationToken = default)
    {
        var podcast = await _context.Podcasts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (podcast is null)
        {
            return UpdatePodcastResult.Failure("Podcast was not found.");
        }

        podcast.Name = request.Name.Trim();
        podcast.Description = request.Description.Trim();

        await _context.SaveChangesAsync(cancellationToken);

        var episodesCount = await _context.Episodes.CountAsync(x => x.PodcastId == id, cancellationToken);

        return UpdatePodcastResult.Success(new PodcastResponse(podcast.Id, podcast.Name, podcast.Description, episodesCount));
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
}