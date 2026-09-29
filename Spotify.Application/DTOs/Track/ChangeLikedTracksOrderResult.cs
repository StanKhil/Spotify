namespace Spotify.Application.DTOs.Track;

public sealed record ChangeLikedTracksOrderResult(
    bool Succeeded,
    IReadOnlyCollection<string> Errors,
    int? OldOrder,
    int? NewOrder)
{
    public static ChangeLikedTracksOrderResult Success(int oldOrder, int newOrder)
        => new(true, [], oldOrder, newOrder);

    public static ChangeLikedTracksOrderResult Failure(params string[] errors)
        => new(false, errors, null, null);
}
