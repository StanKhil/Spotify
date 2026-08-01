namespace Spotify.Application.DTOs.Mood;

public sealed record UpdateMoodResult(
    bool Succeeded,
    MoodResponse? Mood,
    IReadOnlyCollection<string> Errors)
{
    public static UpdateMoodResult Success(MoodResponse mood) =>
        new(true, mood, []);

    public static UpdateMoodResult Failure(params string[] errors) =>
        new(false, null, errors);
}