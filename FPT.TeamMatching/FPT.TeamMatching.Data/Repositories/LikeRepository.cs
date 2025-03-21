using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FPT.TeamMatching.Data.Repositories;

public class LikeRepository : BaseRepository<Like>, ILikeRepository
{
    private readonly FPTMatchingDbContext _dbContext;
    public LikeRepository(FPTMatchingDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> DeleteLikeByBlogId(Guid blogId, Guid userId)
    {
        var like = await _dbContext.Likes
            .Where(e => e.IsDeleted == false &&
                        e.BlogId == blogId &&
                        e.UserId == userId)
            .FirstOrDefaultAsync();

        if (like != null)
        {
            _dbContext.Likes.Remove(like);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        return false;
    }
}