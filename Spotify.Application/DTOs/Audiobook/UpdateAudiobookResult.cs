namespace Spotify.Application.DTOs.Audiobook;

public sealed record UpdateAudiobookResult(
    bool Succeeded,
    AudiobookResponse? Audiobook,
    IReadOnlyCollection<string> Errors)
{
    public static UpdateAudiobookResult Success(AudiobookResponse audiobook) => new(true, audiobook, []);
    public static UpdateAudiobookResult Failure(params string[] errors) => new(false, null, errors);
}