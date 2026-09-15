using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spotify.Application.DTOs.UserProfile;
using Spotify.Application.Interfaces;

namespace Spotify.Web.Controllers
{
    [ApiController]
    [Route("api/user-profile")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [HttpPost("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<UpdateUserProfileResult> UpdateUserProfile([FromBody] UpdateUserProfileRequest request)
        {
            var result = await _userProfileService.UpdateUserProfileAsync(request, CancellationToken.None);
            
            return result;
        }
    }
}
