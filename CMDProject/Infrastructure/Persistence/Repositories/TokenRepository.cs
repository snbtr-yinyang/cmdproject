using CMDProject.Application.Interfaces.Repository;
using CMDProject.Domain.Entities;
using CMDProject.Infrastructure.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;

namespace CMDProject.Infrastructure.Persistence.Repositories
{
    public class TokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public TokenRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(
            UserToken token)
        {
            _context.UserTokens.Add(token);

            await _context.SaveChangesAsync();
        }

        public async Task<UserToken?> GetByTokenAsync(
            string token)
        {
            return await _context.UserTokens
                .FirstOrDefaultAsync(x =>
                    x.TokenName == token);
        }

        public async Task UpdateAsync(
            UserToken token)
        {
            _context.UserTokens.Update(token);

            await _context.SaveChangesAsync();
        }
    }
}
