namespace Spotify.Application.DTOs.Customer;

public sealed record UpdateCustomerResult(
    bool Succeeded,
    CustomerResponse? Customer,
    IReadOnlyCollection<string> Errors)
{
    public static UpdateCustomerResult Success(CustomerResponse customer) => new(true, customer, []);
    public static UpdateCustomerResult Failure(params string[] errors) => new(false, null, errors);
}