namespace Spotify.Application.DTOs.Tag;

public sealed record CreateTagResult(
    bool Succeeded,
    TagResponse? Tag,
    IReadOnlyCollection<string> Errors)
{
    public static CreateTagResult Success(TagResponse tag) =>
        new(true, tag, []);

    public static CreateTagResult Failure(params string[] errors) =>
        new(false, null, errors);
}