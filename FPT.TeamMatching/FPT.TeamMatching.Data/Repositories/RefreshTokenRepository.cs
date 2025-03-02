using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FPT.TeamMatching.Data.Repositories;

public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(FPTMatchingDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

    public async Task<RefreshToken?> GetByRefreshTokenAsync(string refreshToken)
    {
        var key = refreshToken.Trim();
        var queryable = GetQueryable(m => m.Token == key);
        var entity = await queryable.SingleOrDefaultAsync();

        return entity;
    }

    public async Task<RefreshToken?> GetByUserIdAndKeyIdAsync(Guid userId, string kid)
    {
        var queryable = GetQueryable(x =>
            x.UserId == userId &&
            x.KeyId != null &&
            x.KeyId.ToLower() == kid.ToLower()
        );

        return await queryable.SingleOrDefaultAsync();
    }

    public async Task CleanupExpiredTokensAsync()
    {
        try
        {
            var queryable = GetQueryable(token => token.Expiry < DateTime.UtcNow);
            if (!queryable.Any()) return;

            var expiredTokens = await queryable.ToListAsync();

            if (expiredTokens.Count != 0) DeleteRangePermanently(expiredTokens);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while cleaning expired tokens: {ex.Message}");
            throw;
        }
    }
}