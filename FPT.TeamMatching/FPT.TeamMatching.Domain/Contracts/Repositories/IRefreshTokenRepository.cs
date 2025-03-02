using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
{
    Task<RefreshToken?> GetByRefreshTokenAsync(string refreshToken);
    Task<RefreshToken?> GetByUserIdAndKeyIdAsync(Guid userId, string kid);
    Task CleanupExpiredTokensAsync();
}