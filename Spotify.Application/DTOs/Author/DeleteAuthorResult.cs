namespace Spotify.Application.DTOs.Author;

public sealed record DeleteAuthorResult(
    bool Succeeded,
    IReadOnlyCollection<string> Errors)
{
    public static DeleteAuthorResult Success() => new(true, []);
    public static DeleteAuthorResult Failure(params string[] errors) => new(false, errors);
}