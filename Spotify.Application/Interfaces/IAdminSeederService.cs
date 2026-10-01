namespace Spotify.Application.Interfaces;

public interface IAdminSeederService
{
    Task SeedInitialAdminAsync(CancellationToken cancellationToken = default);

    Task SeedCountriesAsync(CancellationToken cancellationToken = default);
}