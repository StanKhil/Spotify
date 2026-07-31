using Spotify.Application.DTOs.Tag;

namespace Spotify.Application.Interfaces;

public interface ITagService
{
    Task<IReadOnlyCollection<TagResponse>> GetTagsAsync(
        CancellationToken cancellationToken = default);

    Task<CreateTagResult> CreateTagAsync(
        CreateTagRequest request,
        CancellationToken cancellationToken = default);

    Task<DeleteTagResult> DeleteTagAsync(
        string id,
        CancellationToken cancellationToken = default);
}