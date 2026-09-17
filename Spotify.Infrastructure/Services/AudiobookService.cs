using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Audiobook;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.Content;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class AudiobookService : IAudiobookService
{
    private readonly ApplicationContext _context;

    public AudiobookService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<AudiobookResponse>> GetAudiobooksAsync(
        CancellationToken cancellationToken = default)
    {
        var audiobooks = await _context.Audiobooks
            .Where(x => x.DeletedAt == null)
            .Include(x => x.AuthorContent)
                .ThenInclude(ac => ac.Authors)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return audiobooks.Select(MapToResponse).ToList();
    }

    public async Task<AudiobookResponse?> GetAudiobookByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var audiobook = await _context.Audiobooks
            .Include(x => x.AuthorContent)
                .ThenInclude(ac => ac.Authors)
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null, cancellationToken);

        return audiobook is null ? null : MapToResponse(audiobook);
    }

    public async Task<CreateAudiobookResult> CreateAudiobookAsync(
        CreateAudiobookRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _context.AudioItems.AnyAsync(x => x.Id == request.AudioItemId, cancellationToken))
        {
            return CreateAudiobookResult.Failure("The specified audio item was not found.");
        }

        if (request.GenreId is string genreId &&
            !await _context.Genres.AnyAsync(x => x.Id == genreId, cancellationToken))
        {
            return CreateAudiobookResult.Failure("The specified genre was not found.");
        }

        var existingAuthorIds = await _context.Authors
            .Where(x => request.AuthorIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var missingAuthors = request.AuthorIds.Except(existingAuthorIds).ToList();
        if (missingAuthors.Count > 0)
        {
            return CreateAudiobookResult.Failure($"The following authors were not found: {string.Join(", ", missingAuthors)}");
        }

        var audiobook = new Audiobook
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            DurationSeconds = 0,
            AudioItemId = request.AudioItemId,
            GenreId = request.GenreId,
            CreatedAt = DateTime.UtcNow
        };

        var authorContent = new AuthorContent
        {
            Id = Guid.NewGuid(),
            Item = audiobook
        };

        foreach (var authorId in existingAuthorIds)
        {
            authorContent.Authors.Add(new AuthorContentAuthor
            {
                AuthorContentId = authorContent.Id,
                AuthorId = authorId
            });
        }

        audiobook.AuthorContentId = authorContent.Id;
        audiobook.AuthorContent = authorContent;

        _context.AuthorContents.Add(authorContent);
        _context.Audiobooks.Add(audiobook);
        await _context.SaveChangesAsync(cancellationToken);

        return CreateAudiobookResult.Success(MapToResponse(audiobook));
    }

    public async Task<UpdateAudiobookResult> EditAudiobookAsync(
        Guid id, UpdateAudiobookRequest request, CancellationToken cancellationToken = default)
    {
        var audiobook = await _context.Audiobooks
            .Include(x => x.AuthorContent)
                .ThenInclude(ac => ac.Authors)
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null, cancellationToken);

        if (audiobook is null)
        {
            return UpdateAudiobookResult.Failure("Audiobook was not found.");
        }

        if (request.GenreId is string genreId &&
            !await _context.Genres.AnyAsync(x => x.Id == genreId, cancellationToken))
        {
            return UpdateAudiobookResult.Failure("The specified genre was not found.");
        }

        var existingAuthorIds = await _context.Authors
            .Where(x => request.AuthorIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var missingAuthors = request.AuthorIds.Except(existingAuthorIds).ToList();
        if (missingAuthors.Count > 0)
        {
            return UpdateAudiobookResult.Failure($"The following authors were not found: {string.Join(", ", missingAuthors)}");
        }

        audiobook.Name = request.Name.Trim();
        audiobook.Description = request.Description?.Trim();
        audiobook.GenreId = request.GenreId;

        var authorsToRemove = audiobook.AuthorContent.Authors
            .Where(x => !existingAuthorIds.Contains(x.AuthorId))
            .ToList();
        foreach (var authorToRemove in authorsToRemove)
        {
            audiobook.AuthorContent.Authors.Remove(authorToRemove);
        }

        var currentAuthorIds = audiobook.AuthorContent.Authors.Select(x => x.AuthorId).ToHashSet();
        foreach (var authorId in existingAuthorIds.Where(authorId => !currentAuthorIds.Contains(authorId)))
        {
            audiobook.AuthorContent.Authors.Add(new AuthorContentAuthor
            {
                AuthorContentId = audiobook.AuthorContentId,
                AuthorId = authorId
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        return UpdateAudiobookResult.Success(MapToResponse(audiobook));
    }

    public async Task<DeleteAudiobookResult> DeleteAudiobookAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var audiobook = await _context.Audiobooks
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null, cancellationToken);

        if (audiobook is null)
        {
            return DeleteAudiobookResult.Failure("Audiobook was not found.");
        }

        var hasEpisodes = await _context.Episodes
            .AnyAsync(x => x.AudiobookId == id && x.DeletedAt == null, cancellationToken);

        if (hasEpisodes)
        {
            return DeleteAudiobookResult.Failure("Cannot delete an audiobook that still has chapters.");
        }

        audiobook.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return DeleteAudiobookResult.Success();
    }

    private static AudiobookResponse MapToResponse(Audiobook audiobook) => new(
        audiobook.Id, audiobook.Name, audiobook.Description, audiobook.DurationSeconds,
        audiobook.GenreId, audiobook.CreatedAt,
        audiobook.AuthorContent.Authors.Select(x => x.AuthorId).ToList());
}