namespace Spotify.Application.DTOs.Customer;

public sealed record DeleteCustomerResult(
    bool Succeeded,
    IReadOnlyCollection<string> Errors)
{
    public static DeleteCustomerResult Success() => new(true, []);
    public static DeleteCustomerResult Failure(params string[] errors) => new(false, errors);
}