using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Author;
using Spotify.Application.Interfaces;
using Spotify.Infrastructure.Persistance.Context;
using Spotify.Domain.Entities.User;

namespace Spotify.Infrastructure.Services;

public sealed class AuthorService : IAuthorService
{
    private const string AuthorRoleName = "Author";

    private readonly ApplicationContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<UserRole> _roleManager;

    public AuthorService(
        ApplicationContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<UserRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IReadOnlyCollection<AuthorResponse>> GetAuthorsAsync(
        CancellationToken cancellationToken = default)
    {
        var authorRole = await _context.Roles.FirstOrDefaultAsync(x => x.Name == AuthorRoleName, cancellationToken);

        if (authorRole is null)
        {
            return [];
        }

        var authorUserIds = await _context.UserRoles
            .Where(x => x.RoleId == authorRole.Id)
            .Select(x => x.UserId)
            .ToListAsync(cancellationToken);

        var authors = await _context.Authors
            .Where(u => authorUserIds.Contains(u.Id))
            .ToListAsync(cancellationToken);

        var result = new List<AuthorResponse>();

        foreach (var author in authors)
        {
            var contentCount = await _context.AuthorContentAuthors
                .CountAsync(x => x.AuthorId == author.Id, cancellationToken);

            result.Add(new AuthorResponse(author.Id, author.Name!, contentCount));
        }

        return result;
    }

    public async Task<AuthorResponse?> GetAuthorByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var author = await _context.Authors.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (author is null || !await _userManager.IsInRoleAsync(author.User! , AuthorRoleName))
        {
            return null;
        }

        var contentCount = await _context.AuthorContentAuthors.CountAsync(x => x.AuthorId == id, cancellationToken);

        return new AuthorResponse(author.Id, author.Name!, contentCount);
    }

    public async Task<CreateAuthorResult> CreateAuthorAsync(
        CreateAuthorRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _context.ApplicationUsers
            .FirstOrDefaultAsync(x => x.Id == request.ApplicationUserId, cancellationToken);

        if (user is null)
        {
            return CreateAuthorResult.Failure("The specified user was not found.");
        }

        if (!await _roleManager.RoleExistsAsync(AuthorRoleName))
        {
            var createRoleResult = await _roleManager.CreateAsync(new UserRole
            {
                Id = Guid.NewGuid(),
                Name = AuthorRoleName,
                Description = "Can upload and manage own content",
                CanCreate = true,
                CanRead = true,
                CanUpdate = true
            });

            if (!createRoleResult.Succeeded)
            {
                return CreateAuthorResult.Failure(createRoleResult.Errors.Select(x => x.Description).ToArray());
            }
        }

        if (await _userManager.IsInRoleAsync(user, AuthorRoleName))
        {
            return CreateAuthorResult.Failure("This user is already an author.");
        }

        var addToRoleResult = await _userManager.AddToRoleAsync(user, AuthorRoleName);

        if (!addToRoleResult.Succeeded)
        {
            return CreateAuthorResult.Failure(addToRoleResult.Errors.Select(x => x.Description).ToArray());
        }

        return CreateAuthorResult.Success(new AuthorResponse(user.Id, user.UserName!, 0));
    }

    public async Task<DeleteAuthorResult> DeleteAuthorAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (user is null || !await _userManager.IsInRoleAsync(user, AuthorRoleName))
        {
            return DeleteAuthorResult.Failure("Author was not found.");
        }

        var hasContent = await _context.AuthorContentAuthors.AnyAsync(x => x.AuthorId == id, cancellationToken);

        if (hasContent)
        {
            return DeleteAuthorResult.Failure("Cannot remove author status while they still have published content.");
        }

        var removeResult = await _userManager.RemoveFromRoleAsync(user, AuthorRoleName);

        if (!removeResult.Succeeded)
        {
            return DeleteAuthorResult.Failure(removeResult.Errors.Select(x => x.Description).ToArray());
        }

        return DeleteAuthorResult.Success();
    }
}