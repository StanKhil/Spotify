namespace Spotify.Application.DTOs.UserProfile
{
    public sealed record UpdateUserProfileResult(
        bool Succeeded,
        IReadOnlyCollection<string> Errors)
    {
        public static UpdateUserProfileResult Success() =>
            new(true, Array.Empty<string>());
        public static UpdateUserProfileResult Failure(params string[] errors) =>
            new(false, errors);
    }
}
