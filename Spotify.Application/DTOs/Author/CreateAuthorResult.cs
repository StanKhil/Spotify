namespace Spotify.Application.DTOs.Author;

public sealed record CreateAuthorResult(
    bool Succeeded,
    AuthorResponse? Author,
    IReadOnlyCollection<string> Errors)
{
    public static CreateAuthorResult Success(AuthorResponse author) => new(true, author, []);
    public static CreateAuthorResult Failure(params string[] errors) => new(false, null, errors);
}