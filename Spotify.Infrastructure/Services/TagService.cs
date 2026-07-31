using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Tag;
using Spotify.Application.Interfaces;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class TagService : ITagService
{
    private readonly ApplicationContext _context;

    public TagService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<TagResponse>> GetTagsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Tags
            .OrderBy(x => x.Id)
            .Select(x => new TagResponse(x.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<CreateTagResult> CreateTagAsync(
        CreateTagRequest request,
        CancellationToken cancellationToken = default)
    {
        var id = request.Id.Trim();

        if (await _context.Tags.AnyAsync(x => x.Id == id, cancellationToken))
        {
            return CreateTagResult.Failure("This tag already exists");
        }

        var tag = new Domain.Entities.Content.Tag { Id = id };

        _context.Tags.Add(tag);
        await _context.SaveChangesAsync(cancellationToken);

        return CreateTagResult.Success(new TagResponse(tag.Id));
    }

    public async Task<DeleteTagResult> DeleteTagAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var tag = await _context.Tags.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (tag is null)
        {
            return DeleteTagResult.Failure("Tag was not found");
        }

        var isInUse = await _context.TrackTags
            .AnyAsync(x => x.TagId == id, cancellationToken);

        if (isInUse)
        {
            return DeleteTagResult.Failure("Cannot delete a tag that is used by existing tracks");
        }

        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync(cancellationToken);

        return DeleteTagResult.Success();
    }
}