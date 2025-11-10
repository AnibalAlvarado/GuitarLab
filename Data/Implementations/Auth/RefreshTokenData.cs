using Data.interfaces.Auth;
using Entity.Contexts;
using Entity.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Implementations.Auth
{
    public class RefreshTokenData: IRefreshTokenData
    {
        private readonly ApplicationDbContext _ctx;

        public RefreshTokenData(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task AddAsync(RefreshToken token)
        {
            _ctx.Set<RefreshToken>().Add(token);
            await _ctx.SaveChangesAsync();
        }

        public Task<RefreshToken?> GetByHashAsync(string tokenHash)
        {
            return _ctx.Set<RefreshToken>()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
        }

        public async Task RevokeAsync(RefreshToken token, string? replacedByTokenHash = null)
        {
            // Traer la entidad en modo tracked
            var tracked = await _ctx.Set<RefreshToken>().FirstOrDefaultAsync(t => t.Id == token.Id);
            if (tracked == null) return;

            tracked.IsRevoked = true;
            tracked.ReplacedByTokenHash = replacedByTokenHash;
            _ctx.Set<RefreshToken>().Update(tracked);
            await _ctx.SaveChangesAsync();
        }

        public Task<IEnumerable<RefreshToken>> GetValidTokensByUserAsync(int userId)
        {
            var now = DateTime.UtcNow;
            var tokens = _ctx.Set<RefreshToken>()
                             .Where(t => t.UserId == userId && !t.IsRevoked && t.ExpiresAt > now)
                             .AsNoTracking()
                             .AsEnumerable();
            return Task.FromResult(tokens);
        }
    }
}
