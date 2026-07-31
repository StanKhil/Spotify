using Spotify.Application.DTOs.Customer;

namespace Spotify.Application.Interfaces;

public interface ICustomerService
{
    Task<IReadOnlyCollection<CustomerResponse>> GetCustomersAsync(CancellationToken cancellationToken = default);
    Task<CustomerResponse?> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UpdateCustomerResult> UpdateCustomerAsync(Guid id, UpdateCustomerRequest request, CancellationToken cancellationToken = default);
    Task<DeleteCustomerResult> DeleteCustomerAsync(Guid id, CancellationToken cancellationToken = default);
}