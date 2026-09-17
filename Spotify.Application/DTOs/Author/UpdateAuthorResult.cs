namespace Spotify.Application.DTOs.Author;

public sealed record UpdateAuthorResult(
    bool Succeeded,
    AuthorResponse? Author,
    IReadOnlyCollection<string> Errors)
{
    public static UpdateAuthorResult Success(AuthorResponse author) => new(true, author, []);

    public static UpdateAuthorResult Failure(params string[] errors) =>
        new(false, null, errors);
}