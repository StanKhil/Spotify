namespace Spotify.Application.DTOs.Audiobook;

public sealed record GetLikedAudiobooksResult(
    bool Succeeded,
    IReadOnlyCollection<string> Errors,
    AudiobookResponseCollection? Audiobooks)
{
    public static GetLikedAudiobooksResult Success(AudiobookResponseCollection audiobooks)
        => new(true, [], audiobooks);

    public static GetLikedAudiobooksResult Failure(params string[] errors)
        => new(false, errors, null);
}
