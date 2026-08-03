using Microsoft.AspNetCore.Mvc;
using Spotify.Application.DTOs.City;
using Spotify.Application.DTOs.Country;
using Spotify.Application.Interfaces;

namespace Spotify.Web.Controllers
{
    [ApiController]
    [Route("api/region")]
    public class RegionController
    {
        private readonly IRegionService _regionService;

        public RegionController(IRegionService regionService) {
            _regionService = regionService;
        }

        [HttpGet("cities")]
        public Task<IEnumerable<CityResponse>> GetAllCities()
        {
            var cities = _regionService.GetAllCitiesAsync().Result;
            return Task.FromResult(cities);
        }

        [HttpGet("countries")]
        public Task<IEnumerable<CountryResponse>> GetAllCountries()
        {
            var countries = _regionService.GetAllCountriesAsync().Result;
            return Task.FromResult(countries);
        }

        [HttpGet("cities/{countryId}")]
        public Task<IEnumerable<CityResponse>> GetCitiesByCountryId(Guid countryId)
        {
            var cities = _regionService.GetCitiesByCountryIdAsync(countryId).Result;
            return Task.FromResult(cities);
        }
    }
}
