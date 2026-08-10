using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Track;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.Content;
using Spotify.Domain.Entities.User;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class TrackActionService : ITrackActionService
{
    private readonly ApplicationContext _context;

    public TrackActionService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<TrackActionResponse?> PlayAsync(
        Guid trackId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var track = await GetTrackAsync(
            trackId,
            cancellationToken);

        if (track is null)
            return null;

        var authorContent = track.Authors.FirstOrDefault();

        if (authorContent is null)
            return null;

        track.PlaysNumber++;

        var history = new ListeningHistory
        {
            Id = Guid.NewGuid(),
            ApplicationUserId = userId,
            AuthorContentId = authorContent.Id,
            ListenedSeconds = 0,
            IsCompleted = false,
            PlayedAt = DateTime.UtcNow
        };

        _context.ListeningHistories.Add(history);

        await _context.SaveChangesAsync(cancellationToken);

        var isLiked = await IsLikedAsync(
            userId,
            authorContent.Id,
            cancellationToken);

        return new TrackActionResponse(
            track.Id,
            track.PlaysNumber,
            isLiked);
    }

    public async Task<TrackActionResponse?> LikeAsync(
        Guid trackId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var track = await GetTrackAsync(
            trackId,
            cancellationToken);

        if (track is null)
            return null;

        var authorContent = track.Authors.FirstOrDefault();

        if (authorContent is null)
            return null;

        var alreadyLiked = await IsLikedAsync(
            userId,
            authorContent.Id,
            cancellationToken);

        if (!alreadyLiked)
        {
            _context.Likes.Add(new Like
            {
                Id = Guid.NewGuid(),
                ApplicationUserId = userId,
                AuthorContentId = authorContent.Id
            });

            await _context.SaveChangesAsync(
                cancellationToken);
        }

        return new TrackActionResponse(
            track.Id,
            track.PlaysNumber,
            true);
    }

    public async Task<TrackActionResponse?> UnlikeAsync(
        Guid trackId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var track = await GetTrackAsync(
            trackId,
            cancellationToken);

        if (track is null)
            return null;

        var authorContent = track.Authors.FirstOrDefault();

        if (authorContent is null)
            return null;

        var like = await _context.Likes
            .FirstOrDefaultAsync(
                x => x.ApplicationUserId == userId &&
                     x.AuthorContentId == authorContent.Id,
                cancellationToken);

        if (like is not null)
        {
            _context.Likes.Remove(like);

            await _context.SaveChangesAsync(
                cancellationToken);
        }

        return new TrackActionResponse(
            track.Id,
            track.PlaysNumber,
            false);
    }

    private async Task<Track?> GetTrackAsync(
        Guid trackId,
        CancellationToken cancellationToken)
    {
        return await _context.Tracks
            .Include(x => x.Authors)
            .FirstOrDefaultAsync(
                x => x.Id == trackId &&
                     x.DeletedAt == null &&
                     !x.IsDraft,
                cancellationToken);
    }

    private async Task<bool> IsLikedAsync(
        Guid userId,
        Guid authorContentId,
        CancellationToken cancellationToken)
    {
        return await _context.Likes
            .AnyAsync(
                x => x.ApplicationUserId == userId &&
                     x.AuthorContentId == authorContentId,
                cancellationToken);
    }
}