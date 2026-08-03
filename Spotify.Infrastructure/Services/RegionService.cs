using Spotify.Application.Interfaces;
using Spotify.Application.DTOs.City;
using Spotify.Application.DTOs.Country;
using Spotify.Infrastructure.Persistance.Context;


namespace Spotify.Infrastructure.Services
{
    public class RegionService : IRegionService
    {
        private readonly ApplicationContext _context;
        public RegionService(ApplicationContext context)
        {
            _context = context;
        }

        public Task<IEnumerable<CityResponse>> GetAllCitiesAsync()
        {
            var cities = _context.Cities.Select(c => new CityResponse(c.Id, c.Name)).ToList();
            return Task.FromResult<IEnumerable<CityResponse>>(cities);
        }

        public Task<IEnumerable<CountryResponse>> GetAllCountriesAsync()
        {
            var countries = _context.Countries.Select(c => new CountryResponse(c.Id, c.Name)).ToList();
            return Task.FromResult<IEnumerable<CountryResponse>>(countries);
        }

        public Task<IEnumerable<CityResponse>> GetCitiesByCountryIdAsync(Guid countryId)
        {
            var cities = _context.Cities.Where(c => c.CountryId == countryId).Select(c => new CityResponse(c.Id, c.Name)).ToList();
            return Task.FromResult<IEnumerable<CityResponse>>(cities);
        }
    }
}
