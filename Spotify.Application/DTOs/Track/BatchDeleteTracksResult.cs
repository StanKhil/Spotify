namespace Spotify.Application.DTOs.Track;

public sealed record BatchDeleteTracksResult(
    bool Succeeded,
    int DeletedCount,
    IReadOnlyCollection<string> Errors)
{
    public static BatchDeleteTracksResult Success(int deletedCount) => new(true, deletedCount, []);
    public static BatchDeleteTracksResult Failure(params string[] errors) => new(false, 0, errors);
}