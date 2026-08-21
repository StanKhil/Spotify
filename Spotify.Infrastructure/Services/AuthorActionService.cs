using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Author;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.User;
using Spotify.Infrastructure.Persistance.Context;
using Spotify.Domain.Entities.Content;

namespace Spotify.Infrastructure.Services;

public sealed class AuthorActionService : IAuthorActionService
{
    private readonly ApplicationContext _context;

    public AuthorActionService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<AuthorActionResponse?> SubscribeAsync(
        Guid authorId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var author = await GetAuthorAsync(
            authorId,
            cancellationToken);

        if (author is null)
            return null;

        if (author.Id == userId)
        {
            return new AuthorActionResponse(
                author.Id,
                await GetSubscriptionsCountAsync(
                    author.Id,
                    cancellationToken),
                false);
        }

        var alreadySubscribed =
            await _context.AuthorSubscriptions
                .AnyAsync(
                    x => x.ApplicationUserId == userId &&
                         x.AuthorId == authorId,
                    cancellationToken);

        if (!alreadySubscribed)
        {
            _context.AuthorSubscriptions.Add(
                new AuthorSubscription
                {
                    Id = Guid.NewGuid(),
                    ApplicationUserId = userId,
                    AuthorId = authorId,
                    CreatedAt = DateTime.UtcNow
                });

            await _context.SaveChangesAsync(
                cancellationToken);
        }

        var subscriptionsCount =
            await GetSubscriptionsCountAsync(
                author.Id,
                cancellationToken);

        return new AuthorActionResponse(
            author.Id,
            subscriptionsCount,
            true);
    }

    public async Task<AuthorActionResponse?> UnsubscribeAsync(
        Guid authorId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var author = await GetAuthorAsync(
            authorId,
            cancellationToken);

        if (author is null)
            return null;

        var subscription =
            await _context.AuthorSubscriptions
                .FirstOrDefaultAsync(
                    x => x.ApplicationUserId == userId &&
                         x.AuthorId == authorId,
                    cancellationToken);

        if (subscription is not null)
        {
            _context.AuthorSubscriptions.Remove(subscription);

            await _context.SaveChangesAsync(
                cancellationToken);
        }

        var subscriptionsCount =
            await GetSubscriptionsCountAsync(
                author.Id,
                cancellationToken);

        return new AuthorActionResponse(
            author.Id,
            subscriptionsCount,
            false);
    }

    private async Task<ApplicationUser?> GetAuthorAsync(
        Guid authorId,
        CancellationToken cancellationToken)
    {
        return await _context.ApplicationUsers
            .FirstOrDefaultAsync(
                x => x.Id == authorId &&
                     x.IsAuthor,
                cancellationToken);
    }

    private async Task<int> GetSubscriptionsCountAsync(
        Guid authorId,
        CancellationToken cancellationToken)
    {
        return await _context.AuthorSubscriptions
            .CountAsync(
                x => x.AuthorId == authorId,
                cancellationToken);
    }
}