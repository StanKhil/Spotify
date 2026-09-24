using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Customer;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.User;
using Spotify.Domain.Enumerations;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class CustomerService : ICustomerService
{
    private readonly ApplicationContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CustomerService(ApplicationContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IReadOnlyCollection<CustomerResponse>> GetCustomersAsync(
        CancellationToken cancellationToken = default)
    {
        var joined = await _context.ApplicationUsers
            .Join(_context.Set<UserProfile>(),
                u => u.Id, p => p.UserId,
                (u, p) => new { User = u, Profile = p })
            .OrderBy(x => x.User.UserName)
            .ToListAsync(cancellationToken);

        var results = new List<CustomerResponse>(joined.Count);

        foreach (var item in joined)
        {
            var roles = await _userManager.GetRolesAsync(item.User);
            var role = roles.FirstOrDefault() ?? "Self-Registered";

            results.Add(new CustomerResponse(
                item.User.Id, item.User.Email!, item.User.UserName!,
                item.Profile.CountryId, item.Profile.CityId,
                item.Profile.Birthdate, item.Profile.IsAdult,
                item.Profile.RegisteredAt, item.Profile.DeletedAt, role));
        }

        return results;
    }

    public async Task<CustomerResponse?> GetCustomerByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _context.ApplicationUsers
            .Where(u => u.Id == id)
            .Join(_context.Set<UserProfile>(),
                u => u.Id, p => p.UserId,
                (u, p) => new { User = u, Profile = p })
            .FirstOrDefaultAsync(cancellationToken);

        if (item is null)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(item.User);
        var role = roles.FirstOrDefault() ?? "Self-Registered";

        return new CustomerResponse(
            item.User.Id, item.User.Email!, item.User.UserName!,
            item.Profile.CountryId, item.Profile.CityId,
            item.Profile.Birthdate, item.Profile.IsAdult,
            item.Profile.RegisteredAt, item.Profile.DeletedAt, role);
    }

    public async Task<CreateCustomerResult> CreateCustomerAsync(
        CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _context.Countries.AnyAsync(x => x.Id == request.CountryId, cancellationToken))
        {
            return CreateCustomerResult.Failure("The specified country was not found.");
        }

        if (!await _context.Cities.AnyAsync(x => x.Id == request.CityId && x.CountryId == request.CountryId, cancellationToken))
        {
            return CreateCustomerResult.Failure("The specified city was not found in the given country.");
        }

        if (request.Role != "Admin" && request.Role != "Self-Registered")
        {
            return CreateCustomerResult.Failure("Role must be either 'Admin' or 'Self-Registered'.");
        }

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return CreateCustomerResult.Failure("A user with this email already exists.");
        }

        var defaultSubscription = await _context.Set<Subscription>()
            .FirstOrDefaultAsync(x => x.Name == "Free", cancellationToken)
            ?? await _context.Set<Subscription>()
                .OrderBy(x => x.Price)
                .FirstOrDefaultAsync(cancellationToken);

        if (defaultSubscription is null)
        {
            return CreateCustomerResult.Failure("No subscription plans exist in the system yet. Seed a default plan first.");
        }

        var settings = new Settings
        {
            Id = Guid.NewGuid(),
            Language = Language.English
        };
        _context.Set<Settings>().Add(settings);

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = request.Email.Trim(),
            UserName = request.UserName.Trim(),
            EmailConfirmed = true,
            SettingsId = settings.Id,
            SubscriptionId = defaultSubscription.Id
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            return CreateCustomerResult.Failure(createResult.Errors.Select(e => e.Description).ToArray());
        }

        var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
        if (!roleResult.Succeeded)
        {
            return CreateCustomerResult.Failure(roleResult.Errors.Select(e => e.Description).ToArray());
        }

        var now = DateTime.UtcNow;
        var isAdult = request.Birthdate.AddYears(18) <= DateOnly.FromDateTime(now);

        var profile = new UserProfile
        {
            UserId = user.Id,
            CountryId = request.CountryId,
            CityId = request.CityId,
            Birthdate = request.Birthdate.ToDateTime(TimeOnly.MinValue),
            IsAdult = isAdult,
            RegisteredAt = now,
            DeletedAt = null
        };

        _context.Set<UserProfile>().Add(profile);
        await _context.SaveChangesAsync(cancellationToken);

        return CreateCustomerResult.Success(new CustomerResponse(
            user.Id, user.Email!, user.UserName!, profile.CountryId, profile.CityId,
            profile.Birthdate, profile.IsAdult, profile.RegisteredAt, profile.DeletedAt, request.Role));
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

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Self-Registered";

        return UpdateCustomerResult.Success(new CustomerResponse(
            user.Id, user.Email!, user.UserName!, profile.CountryId, profile.CityId,
            profile.Birthdate, profile.IsAdult, profile.RegisteredAt, profile.DeletedAt, role));
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