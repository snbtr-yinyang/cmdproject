using CMDProject.Domain.Entities;

namespace CMDProject.Application.Interfaces.Repository
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(UserToken token);

        Task<UserToken?> GetByTokenAsync(string token);

        Task UpdateAsync(UserToken token);
    }
}
