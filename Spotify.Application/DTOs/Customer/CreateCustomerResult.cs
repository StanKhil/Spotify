namespace Spotify.Application.DTOs.Customer;

public sealed class CreateCustomerResult
{
    public bool Succeeded { get; }
    public CustomerResponse? Customer { get; }
    public IReadOnlyCollection<string> Errors { get; }

    private CreateCustomerResult(bool succeeded, CustomerResponse? customer, IReadOnlyCollection<string> errors)
    {
        Succeeded = succeeded;
        Customer = customer;
        Errors = errors;
    }

    public static CreateCustomerResult Success(CustomerResponse customer) =>
        new(true, customer, Array.Empty<string>());

    public static CreateCustomerResult Failure(params string[] errors) =>
        new(false, null, errors);
}