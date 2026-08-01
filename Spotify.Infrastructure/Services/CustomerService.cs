using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Customer;
using Spotify.Application.Interfaces;
using Spotify.Infrastructure.Persistance.Context;
using Spotify.Domain.Entities.User;

namespace Spotify.Infrastructure.Services;

public sealed class CustomerService : ICustomerService
{
    private readonly ApplicationContext _context;

    public CustomerService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<CustomerResponse>> GetCustomersAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.ApplicationUsers
            .Join(_context.Set<UserProfile>(),
                u => u.Id, p => p.UserId,
                (u, p) => new CustomerResponse(
                    u.Id, u.Email!, u.UserName!, p.CountryId, p.CityId,
                    p.Birthdate, p.IsAdult, p.RegisteredAt, p.DeletedAt))
            .OrderBy(x => x.UserName)
            .ToListAsync(cancellationToken);
    }

    public async Task<CustomerResponse?> GetCustomerByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ApplicationUsers
            .Where(u => u.Id == id)
            .Join(_context.Set<UserProfile>(),
                u => u.Id, p => p.UserId,
                (u, p) => new CustomerResponse(
                    u.Id, u.Email!, u.UserName!, p.CountryId, p.CityId,
                    p.Birthdate, p.IsAdult, p.RegisteredAt, p.DeletedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UpdateCustomerResult> UpdateCustomerAsync(
        Guid id, UpdateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        var profile = await _context.Set<UserProfile>()
            .FirstOrDefaultAsync(x => x.UserId == id, cancellationToken);

        if (user is null || profile is null)
        {
            return UpdateCustomerResult.Failure("Customer was not found.");
        }

        if (!await _context.Countries.AnyAsync(x => x.Id == request.CountryId, cancellationToken))
        {
            return UpdateCustomerResult.Failure("The specified country was not found.");
        }

        if (!await _context.Cities.AnyAsync(x => x.Id == request.CityId && x.CountryId == request.CountryId, cancellationToken))
        {
            return UpdateCustomerResult.Failure("The specified city was not found in the given country.");
        }

        user.UserName = request.UserName.Trim();
        profile.CountryId = request.CountryId;
        profile.CityId = request.CityId;
        profile.Birthdate = request.Birthdate.ToDateTime(TimeOnly.MinValue);
        profile.IsAdult = request.Birthdate.AddYears(18) <= DateOnly.FromDateTime(DateTime.UtcNow);

        await _context.SaveChangesAsync(cancellationToken);

        return UpdateCustomerResult.Success(new CustomerResponse(
            user.Id, user.Email!, user.UserName!, profile.CountryId, profile.CityId,
            profile.Birthdate, profile.IsAdult, profile.RegisteredAt, profile.DeletedAt));
    }

    public async Task<DeleteCustomerResult> DeleteCustomerAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var profile = await _context.Set<UserProfile>()
            .FirstOrDefaultAsync(x => x.UserId == id, cancellationToken);

        if (profile is null)
        {
            return DeleteCustomerResult.Failure("Customer was not found.");
        }

        profile.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return DeleteCustomerResult.Success();
    }
}