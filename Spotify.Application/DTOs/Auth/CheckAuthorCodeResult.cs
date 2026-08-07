
namespace Spotify.Application.DTOs.Auth
{
    public sealed record CheckAuthorCodeResult(
        bool Succeeded,
        IReadOnlyCollection<string> Errors)
    {
        public static CheckAuthorCodeResult Success() =>
            new(true, []);

        public static CheckAuthorCodeResult Failure(params string[] errors) =>
            new(false, errors);
    }
}
