using Spotify.Application.Interfaces;
using Spotify.Infrastructure.Persistance.Context;
using Microsoft.EntityFrameworkCore;

namespace Spotify.Infrastructure.Services
{
    public sealed class TokenRevocationService : ITokenRevocationService
    {
        private readonly ApplicationContext _context;

        public TokenRevocationService(ApplicationContext context)
        {
            _context = context;
        }

        public Task<bool> IsRevokedAsync(
            string jti,
            CancellationToken cancellationToken = default)
        {
            return _context.RevokedTokens
                .AnyAsync(x => x.Jti == jti, cancellationToken);
        }
    }
}
