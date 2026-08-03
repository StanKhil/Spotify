using Spotify.Application.DTOs.City;
using Spotify.Application.DTOs.Country;

namespace Spotify.Application.Interfaces
{
    public interface IRegionService
    {
        Task<IEnumerable<CityResponse>> GetAllCitiesAsync();
        Task<IEnumerable<CountryResponse>> GetAllCountriesAsync();
        Task<IEnumerable<CityResponse>> GetCitiesByCountryIdAsync(Guid countryId);
    }
}
