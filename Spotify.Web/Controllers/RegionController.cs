using Microsoft.AspNetCore.Mvc;
using Spotify.Application.DTOs.City;
using Spotify.Application.DTOs.Country;
using Spotify.Application.Interfaces;

namespace Spotify.Web.Controllers
{
    [ApiController]
    [Route("api/region")]
    public class RegionController : ControllerBase
    {
        private readonly IRegionService _regionService;

        public RegionController(IRegionService regionService)
        {
            _regionService = regionService;
        }

        
        [HttpGet("cities")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllCities()
        {
            var cities = await _regionService.GetAllCitiesAsync();

            if (cities == null || !cities.Any())
            {
                return NotFound(new { message = "No cities found." });
            }

            return Ok(cities);
        }

       
        [HttpGet("countries")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllCountries()
        {
            var countries = await _regionService.GetAllCountriesAsync();

            if (countries == null || !countries.Any())
            {
                return NotFound(new { message = "No countries found." });
            }

            return Ok(countries);
        }

       
        [HttpGet("cities/{countryId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCitiesByCountryId(Guid countryId)
        {
            if (countryId == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid country ID." });
            }

            var cities = await _regionService.GetCitiesByCountryIdAsync(countryId);

            if (cities == null || !cities.Any())
            {
                return NotFound(new { message = $"No cities found for country ID: {countryId}" });
            }

            return Ok(cities);
        }
    }
}
