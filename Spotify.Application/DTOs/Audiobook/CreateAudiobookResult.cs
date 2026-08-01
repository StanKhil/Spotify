namespace Spotify.Application.DTOs.Audiobook;

public sealed record CreateAudiobookResult(
    bool Succeeded,
    AudiobookResponse? Audiobook,
    IReadOnlyCollection<string> Errors)
{
    public static CreateAudiobookResult Success(AudiobookResponse audiobook) => new(true, audiobook, []);
    public static CreateAudiobookResult Failure(params string[] errors) => new(false, null, errors);
}