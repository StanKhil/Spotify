namespace Spotify.Application.DTOs.Mood;

public sealed record DeleteMoodResult(
    bool Succeeded,
    IReadOnlyCollection<string> Errors)
{
    public static DeleteMoodResult Success() => new(true, []);

    public static DeleteMoodResult Failure(params string[] errors) =>
        new(false, errors);
}