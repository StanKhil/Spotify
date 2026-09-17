using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Author;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.Content;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class AuthorService : IAuthorService
{
    private readonly ApplicationContext _context;

    public AuthorService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<AuthorResponse>> GetAuthorsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Authors
            .OrderBy(x => x.Name)
            .Select(x => new AuthorResponse(
                x.Id,
                x.Name,
                x.MonthList,
                x.Bio,
                x.BioImageItemId,
                x.AuthoredContent.Count))
            .ToListAsync(cancellationToken);
    }

    public async Task<AuthorResponse?> GetAuthorByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Authors
            .Where(x => x.Id == id)
            .Select(x => new AuthorResponse(
                x.Id,
                x.Name,
                x.MonthList,
                x.Bio,
                x.BioImageItemId,
                x.AuthoredContent.Count))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CreateAuthorResult> CreateAuthorAsync(
        CreateAuthorRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.BioImageItemId is Guid bioImageItemId &&
            !await _context.ImageItems.AnyAsync(x => x.Id == bioImageItemId, cancellationToken))
        {
            return CreateAuthorResult.Failure("The specified bio image was not found.");
        }

        if (request.ApplicationUserId is Guid applicationUserId)
        {
            var userExists = await _context.ApplicationUsers
                .AnyAsync(x => x.Id == applicationUserId, cancellationToken);

            if (!userExists)
            {
                return CreateAuthorResult.Failure("The specified user was not found.");
            }

            var userAlreadyLinked = await _context.Authors
                .AnyAsync(x => x.UserId == applicationUserId, cancellationToken);

            if (userAlreadyLinked)
            {
                return CreateAuthorResult.Failure("The specified user is already linked to an author.");
            }
        }

        var author = new Author
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            MonthList = request.MonthList,
            Bio = request.Bio?.Trim(),
            BioImageItemId = request.BioImageItemId,
            UserId = request.ApplicationUserId
        };

        _context.Authors.Add(author);
        await _context.SaveChangesAsync(cancellationToken);

        return CreateAuthorResult.Success(MapToResponse(author, 0));
    }

    public async Task<UpdateAuthorResult> UpdateAuthorAsync(
        Guid id,
        UpdateAuthorRequest request,
        CancellationToken cancellationToken = default)
    {
        var author = await _context.Authors
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (author is null)
        {
            return UpdateAuthorResult.Failure("Author was not found.");
        }

        if (request.BioImageItemId is Guid bioImageItemId &&
            !await _context.ImageItems.AnyAsync(x => x.Id == bioImageItemId, cancellationToken))
        {
            return UpdateAuthorResult.Failure("The specified bio image was not found.");
        }

        author.Name = request.Name.Trim();
        author.MonthList = request.MonthList;
        author.Bio = request.Bio?.Trim();
        author.BioImageItemId = request.BioImageItemId;

        await _context.SaveChangesAsync(cancellationToken);

        var contentCount = await _context.AuthorContentAuthors
            .CountAsync(x => x.AuthorId == author.Id, cancellationToken);

        return UpdateAuthorResult.Success(MapToResponse(author, contentCount));
    }

    public async Task<DeleteAuthorResult> DeleteAuthorAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var author = await _context.Authors
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (author is null)
        {
            return DeleteAuthorResult.Failure("Author was not found.");
        }

        var hasContent = await _context.AuthorContentAuthors
            .AnyAsync(x => x.AuthorId == id, cancellationToken);

        if (hasContent)
        {
            return DeleteAuthorResult.Failure(
                "Cannot delete an author who still has published content.");
        }

        _context.Authors.Remove(author);
        await _context.SaveChangesAsync(cancellationToken);

        return DeleteAuthorResult.Success();
    }

    private static AuthorResponse MapToResponse(Author author, int contentCount) =>
        new(
            author.Id,
            author.Name,
            author.MonthList,
            author.Bio,
            author.BioImageItemId,
            contentCount);
}