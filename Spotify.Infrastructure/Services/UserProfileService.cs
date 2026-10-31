using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.UserProfile;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.User;
using Spotify.Infrastructure.Persistance.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Infrastructure.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly ApplicationContext _context;
        private readonly ICurrentUserService _currentUserService;   
        public UserProfileService(ApplicationContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<UserProfileStatisticsResult> GetUserProfileStatisticsAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = _context.ApplicationUsers
                .Include(au => au.Profile)
                .Include(au => au.Author)
                    .ThenInclude(a => a.Followers)
                .Include(au => au.Author)
                    .ThenInclude(a => a.AuthoredContent)
                        .ThenInclude(aca => aca.AuthorContent)
                            .ThenInclude(ac => ac.Item)
                .FirstOrDefault(au => au.Id == userId);

            if (user is null)
            {
                return UserProfileStatisticsResult.Failure("User not found.");
            }

            if(user.Author is null)
            {
                return UserProfileStatisticsResult.Failure("User is not an author.");
            }

            long totalListenersCount = user.Author.AuthoredContent
    .Sum(aca => aca.AuthorContent?.Item?.PlaysNumber ?? 0);

            var statistics = new UserProfileStatisticsResponse(
                TotalSubsrcibesCount: user.Author.Followers.Count,
                TotalContentCount: user.Author.AuthoredContent.Count,
                TotalListenersCount: totalListenersCount
            );

            return UserProfileStatisticsResult.Success(statistics);
        }

        public async Task<UpdateUserProfileResult> UpdateUserProfileAsync(UpdateUserProfileRequest request, CancellationToken cancellationToken)
        {
            if(_currentUserService.UserId != request.userId)
            {
                return UpdateUserProfileResult.Failure("You can only update your own profile.");
            }

            var user = await _context
                .ApplicationUsers
                .Include(au => au.Profile)
                .FirstOrDefaultAsync(
                    au => au.Id == request.userId,
                    cancellationToken);

            if(user is null)
            {
                return UpdateUserProfileResult.Failure("User not found.");
            }

            if(!string.IsNullOrWhiteSpace(request.userName))
            {
                user.UserName = request.userName;
            }
            if(request.description != null)
            {
                user.Profile.Description = request.description;
            }
            if(!string.IsNullOrWhiteSpace(request.avatarImageId))
            {
                user.Profile.AvatarImageId = Guid.Parse(request.avatarImageId);
            }
            if(!string.IsNullOrWhiteSpace(request.coverImageId))
            {
                user.Profile.CoverImageId = Guid.Parse(request.coverImageId);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return UpdateUserProfileResult.Success();
        }
    }
}
