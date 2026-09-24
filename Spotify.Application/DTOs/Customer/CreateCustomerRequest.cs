namespace Spotify.Application.DTOs.Customer;

public sealed record CreateCustomerRequest(
    string Email,
    string UserName,
    string Password,
    Guid CountryId,
    Guid CityId,
    DateOnly Birthdate,
    string Role);