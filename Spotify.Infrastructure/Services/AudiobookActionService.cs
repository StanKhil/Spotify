using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Audiobook;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.Content;
using Spotify.Domain.Entities.User;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class AudiobookActionService : IAudiobookActionService
{
    private readonly ApplicationContext _context;

    public AudiobookActionService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<AudiobookActionResponse?> PlayAsync(
        Guid audiobookId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var audiobook = await GetAudiobookAsync(audiobookId, cancellationToken);
        if (audiobook is null)
            return null;

        _context.ListeningHistories.Add(new ListeningHistory
        {
            Id = Guid.NewGuid(),
            ApplicationUserId = userId,
            AuthorContentId = audiobook.AuthorContentId,
            ListenedSeconds = 0,
            IsCompleted = false,
            PlayedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);

        return new AudiobookActionResponse(
            audiobook.Id,
            await IsLikedAsync(userId, audiobook.AuthorContentId, cancellationToken));
    }

    public async Task<AudiobookActionResponse?> LikeAsync(
        Guid audiobookId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var audiobook = await GetAudiobookAsync(audiobookId, cancellationToken);
        if (audiobook is null)
            return null;

        if (!await IsLikedAsync(userId, audiobook.AuthorContentId, cancellationToken))
        {
            _context.Likes.Add(new Like
            {
                Id = Guid.NewGuid(),
                ApplicationUserId = userId,
                AuthorContentId = audiobook.AuthorContentId
            });

            await _context.SaveChangesAsync(cancellationToken);
        }

        return new AudiobookActionResponse(audiobook.Id, true);
    }

    public async Task<AudiobookActionResponse?> UnlikeAsync(
        Guid audiobookId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var audiobook = await GetAudiobookAsync(audiobookId, cancellationToken);
        if (audiobook is null)
            return null;

        var like = await _context.Likes.FirstOrDefaultAsync(
            x => x.ApplicationUserId == userId &&
                 x.AuthorContentId == audiobook.AuthorContentId,
            cancellationToken);

        if (like is not null)
        {
            _context.Likes.Remove(like);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return new AudiobookActionResponse(audiobook.Id, false);
    }

    public async Task<GetLikedAudiobooksResult> GetLikedAudiobooksAsync(
        int maxPerPage,
        int page,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (maxPerPage <= 0 || page <= 0)
            return GetLikedAudiobooksResult.Failure("Invalid pagination parameters.");

        var likedAudiobooksQuery = _context.Likes
            .AsNoTracking()
            .Where(x => x.ApplicationUserId == userId)
            .Select(x => x.AuthorContent.Item)
            .OfType<Audiobook>()
            .Where(x => x.DeletedAt == null);

        var totalCount = await likedAudiobooksQuery.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling((double)totalCount / maxPerPage);

        var audiobooks = await likedAudiobooksQuery
            .Include(x => x.AuthorContent)
                .ThenInclude(x => x.Authors)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * maxPerPage)
            .Take(maxPerPage)
            .ToListAsync(cancellationToken);

        var responses = audiobooks.Select(MapToResponse).ToList();

        return GetLikedAudiobooksResult.Success(
            new AudiobookResponseCollection(responses, totalCount, totalPages));
    }

    private Task<Audiobook?> GetAudiobookAsync(
        Guid audiobookId,
        CancellationToken cancellationToken)
    {
        return _context.Audiobooks.FirstOrDefaultAsync(
            x => x.Id == audiobookId && x.DeletedAt == null,
            cancellationToken);
    }

    private Task<bool> IsLikedAsync(
        Guid userId,
        Guid authorContentId,
        CancellationToken cancellationToken)
    {
        return _context.Likes.AnyAsync(
            x => x.ApplicationUserId == userId && x.AuthorContentId == authorContentId,
            cancellationToken);
    }

    private static AudiobookResponse MapToResponse(Audiobook audiobook) => new(
        audiobook.Id,
        audiobook.Name,
        audiobook.Description,
        audiobook.DurationSeconds,
        audiobook.GenreId,
        audiobook.CreatedAt,
        audiobook.AuthorContent.Authors.Select(x => x.AuthorId).ToList());
}
