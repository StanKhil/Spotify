using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Auth;
using Spotify.Application.Interfaces;
using Spotify.Infrastructure.Authentication;
using Spotify.Domain.Entities.Content;
using Spotify.Domain.Enumerations;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class AuthenticationService : IAuthenticationService
{
    private const string DefaultRoleName = "User";

    private readonly ApplicationContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<UserRole> _roleManager;
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public AuthenticationService(
        ApplicationContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<UserRole> roleManager,
        JwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<RegisterResult> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim();
        var userName = request.UserName.Trim();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(userName))
        {
            return RegisterResult.Failure("Email and user name are required.");
        }

        if (request.Birthdate > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return RegisterResult.Failure("Birthdate cannot be in the future.");
        }

        if (await _userManager.FindByEmailAsync(email) is not null)
        {
            return RegisterResult.Failure("A user with this email already exists.");
        }

        if (await _userManager.FindByNameAsync(userName) is not null)
        {
            return RegisterResult.Failure("This user name is already taken.");
        }

        var countryExists = await _context.Countries
            .AnyAsync(x => x.Id == request.CountryId, cancellationToken);
        var cityMatchesCountry = await _context.Cities
            .AnyAsync(x => x.Id == request.CityId && x.CountryId == request.CountryId, cancellationToken);
        var subscriptionExists = await _context.Subscriptions
            .AnyAsync(x => x.Id == request.SubscriptionId, cancellationToken);

        if (!countryExists || !cityMatchesCountry || !subscriptionExists)
        {
            return RegisterResult.Failure("Country, city, or subscription was not found.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        var settings = new Settings
        {
            Id = Guid.NewGuid(),
            Language = Language.English
        };

        _context.Settings.Add(settings);

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = userName,
            SubscriptionId = request.SubscriptionId,
            SettingsId = settings.Id
        };

        var createUserResult = await _userManager.CreateAsync(user, request.Password);
        if (!createUserResult.Succeeded)
        {
            await transaction.RollbackAsync(cancellationToken);
            return RegisterResult.Failure(createUserResult.Errors.Select(x => x.Description).ToArray());
        }

        if (!await _roleManager.RoleExistsAsync(DefaultRoleName))
        {
            var createRoleResult = await _roleManager.CreateAsync(new UserRole
            {
                Id = Guid.NewGuid(),
                Name = DefaultRoleName,
                Description = "Default listener role",
                CanRead = true
            });

            if (!createRoleResult.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                return RegisterResult.Failure(createRoleResult.Errors.Select(x => x.Description).ToArray());
            }
        }

        var addToRoleResult = await _userManager.AddToRoleAsync(user, DefaultRoleName);
        if (!addToRoleResult.Succeeded)
        {
            await transaction.RollbackAsync(cancellationToken);
            return RegisterResult.Failure(addToRoleResult.Errors.Select(x => x.Description).ToArray());
        }

        _context.UserProfiles.Add(new UserProfile
        {
            UserId = user.Id,
            CountryId = request.CountryId,
            CityId = request.CityId,
            Birthdate = request.Birthdate.ToDateTime(TimeOnly.MinValue),
            IsAdult = request.Birthdate.AddYears(18) <= DateOnly.FromDateTime(DateTime.UtcNow),
            RegisteredAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return RegisterResult.Success(new RegisterResponse(user.Id, user.Email, user.UserName));
    }

    public async Task<LoginResult> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim();
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return LoginResult.Failure("Invalid email or password.");
        }

        var isDeleted = await _context.UserProfiles
            .AnyAsync(x => x.UserId == user.Id && x.DeletedAt != null, cancellationToken);

        if (isDeleted)
        {
            return LoginResult.Failure("Invalid email or password.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        return LoginResult.Success(_jwtTokenGenerator.Create(user, roles));
    }
}
