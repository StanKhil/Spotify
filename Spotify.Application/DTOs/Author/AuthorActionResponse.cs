namespace Spotify.Application.DTOs.Author
{
    public sealed record AuthorActionResponse(
    Guid AuthorId,
    int SubscriptionsCount,
    bool IsSubscribed);
}
