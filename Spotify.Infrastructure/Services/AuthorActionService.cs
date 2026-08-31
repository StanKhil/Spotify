using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Author;
using Spotify.Application.DTOs.Track;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.Content;
using Spotify.Domain.Entities.User;
using Spotify.Infrastructure.Persistance.Context;

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

    public async Task<SubscribedAuthorsResult> GetSubscribed(
        int maxPerPage,
        int page,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (maxPerPage <= 0 || page <= 0)
        {
            return SubscribedAuthorsResult.Failure(
                "Invalid pagination parameters.");
        }

        var subscribedAuthorsQuery = _context.AuthorSubscriptions
            .Where(a => a.ApplicationUserId == userId);
            
        var totalSubscribedAuthors = await subscribedAuthorsQuery.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling((double)totalSubscribedAuthors / maxPerPage);

        var subscriedAuthors = await subscribedAuthorsQuery
            .OrderByDescending(s => s.CreatedAt)
            .Include(s => s.Author)
            .Skip((page - 1) * maxPerPage)
            .Take(maxPerPage)
            .ToListAsync(cancellationToken);

        var response = new List<AuthorResponse>();
        foreach(var authorSub in subscriedAuthors)
        {
            response.Add(new AuthorResponse(authorSub.AuthorId, 
                authorSub.Author.Email!, 
                authorSub.Author.UserName!, 
                authorSub.Author.AuthoredContent.Count));
        }

        return SubscribedAuthorsResult.Success(
            new SubscribedAuthorsResponse(
                response));
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