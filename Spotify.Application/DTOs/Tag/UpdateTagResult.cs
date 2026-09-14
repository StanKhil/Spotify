namespace Spotify.Application.DTOs.Tag;

public sealed record UpdateTagResult(
    bool Succeeded,
    TagResponse? Tag,
    IReadOnlyCollection<string> Errors)
{
    public static UpdateTagResult Success(TagResponse tag) => new(true, tag, []);
    public static UpdateTagResult Failure(params string[] errors) => new(false, null, errors);
}