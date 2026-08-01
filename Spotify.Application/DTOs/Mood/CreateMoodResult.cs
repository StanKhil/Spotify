namespace Spotify.Application.DTOs.Mood;

public sealed record CreateMoodResult(
    bool Succeeded,
    MoodResponse? Mood,
    IReadOnlyCollection<string> Errors)
{
    public static CreateMoodResult Success(MoodResponse mood) =>
        new(true, mood, []);

    public static CreateMoodResult Failure(params string[] errors) =>
        new(false, null, errors);
}