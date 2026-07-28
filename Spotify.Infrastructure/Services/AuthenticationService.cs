using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Spotify.Application.DTOs.Auth;
using Spotify.Application.DTOs.ForgotPassword;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.Content;
using Spotify.Domain.Enumerations;
using Spotify.Infrastructure.Authentication;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class AuthenticationService : IAuthenticationService
{
    private const string DefaultRoleName = "User";
    private static readonly TimeSpan PasswordResetCodeLifetime = TimeSpan.FromMinutes(10);

    private readonly ApplicationContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<UserRole> _roleManager;
    private readonly JwtTokenGenerator _jwtTokenGenerator;
    private readonly IEmailService _emailService;
    private readonly IMemoryCache _cache;

    public AuthenticationService(
        ApplicationContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<UserRole> roleManager,
        JwtTokenGenerator jwtTokenGenerator,
        IEmailService emailService,
        IMemoryCache cache)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _emailService = emailService;
        _cache = cache;
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

    public async Task<CheckEmailResult> CheckEmailAsync(
        CheckEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim();
        var user = await _userManager.FindByEmailAsync(email);

        if (user?.Email is null)
        {
            return CheckEmailResult.Success();
        }

        var code = RandomNumberGenerator.GetInt32(100_000, 1_000_000).ToString();
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var cacheKey = GetPasswordResetCacheKey(user.Email);
        var expiresAtUtc = DateTimeOffset.UtcNow.Add(PasswordResetCodeLifetime);

        _cache.Set(
            cacheKey,
            new PasswordResetCodeEntry(HashCode(code), resetToken, false, expiresAtUtc),
            expiresAtUtc);

        try
        {
            await _emailService.SendAsync(
                user.Email,
                "Spotify password reset code",
                $"<p>Your password reset code is <strong>{code}</strong>.</p><p>It expires in 10 minutes.</p>",
                cancellationToken);
        }
        catch (Exception)
        {
            _cache.Remove(cacheKey);
            return CheckEmailResult.Failure("Unable to send the password reset email. Please try again later.");
        }

        return CheckEmailResult.Success();
    }

    public async Task<CheckCodeResult> CheckCodeAsync(
        CheckCodeRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim();
        var user = await _userManager.FindByEmailAsync(email);

        if (user?.Email is not string userEmail ||
            !_cache.TryGetValue<PasswordResetCodeEntry>(GetPasswordResetCacheKey(userEmail), out var entry) ||
            entry is null ||
            entry.ExpiresAtUtc <= DateTimeOffset.UtcNow ||
            !CodesMatch(entry.CodeHash, request.Code))
        {
            return CheckCodeResult.Failure("The password reset code is invalid or expired.");
        }

        _cache.Set(
            GetPasswordResetCacheKey(userEmail),
            entry with { IsVerified = true },
            entry.ExpiresAtUtc);

        return CheckCodeResult.Success();
    }

    public async Task<NewPasswordResult> NewPasswordAsync(
        NewPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim();
        var user = await _userManager.FindByEmailAsync(email);

        if (user?.Email is not string userEmail ||
            !_cache.TryGetValue<PasswordResetCodeEntry>(GetPasswordResetCacheKey(userEmail), out var entry) ||
            entry is null ||
            !entry.IsVerified)
        {
            return NewPasswordResult.Failure("Confirm the password reset code before setting a new password.");
        }

        var resetResult = await _userManager.ResetPasswordAsync(user, entry.ResetToken, request.Password);
        if (!resetResult.Succeeded)
        {
            return NewPasswordResult.Failure(resetResult.Errors.Select(x => x.Description).ToArray());
        }

        _cache.Remove(GetPasswordResetCacheKey(userEmail));
        return NewPasswordResult.Success();
    }

    private static string GetPasswordResetCacheKey(string email) =>
        $"password-reset:{email.Trim().ToUpperInvariant()}";

    private static string HashCode(string code) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(code)));

    private static bool CodesMatch(string expectedHash, string code)
    {
        var actualHash = HashCode(code);
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(expectedHash),
            Encoding.UTF8.GetBytes(actualHash));
    }

    private sealed record PasswordResetCodeEntry(
        string CodeHash,
        string ResetToken,
        bool IsVerified,
        DateTimeOffset ExpiresAtUtc);
}
