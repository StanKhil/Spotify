namespace Spotify.Application.DTOs.Media;

public sealed record MediaUploadResult(
    bool Succeeded,
    Guid? ItemId,
    string? Url,
    IReadOnlyCollection<string> Errors)
{
    public static MediaUploadResult Success(Guid itemId, string url) => new(true, itemId, url, []);
    public static MediaUploadResult Failure(params string[] errors) => new(false, null, null, errors);
}