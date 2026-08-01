namespace Spotify.Application.DTOs.Audiobook;

public sealed record DeleteAudiobookResult(
    bool Succeeded,
    IReadOnlyCollection<string> Errors)
{
    public static DeleteAudiobookResult Success() => new(true, []);
    public static DeleteAudiobookResult Failure(params string[] errors) => new(false, errors);
}