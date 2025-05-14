using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ZstdSharp.Unsafe;

namespace FPT.TeamMatching.Data.Repositories;

public class ReviewRepository : BaseRepository<Review>, IReviewRepository
{
    private readonly FPTMatchingDbContext _dbContext;
    public ReviewRepository(FPTMatchingDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Review>?> GetByProjectId(Guid projectId)
    {
        var reviews = await _dbContext.Reviews
                            .Where(e => e.ProjectId == projectId && e.IsDeleted == false)
                            .Include(x => x.Reviewer1)
                            .Include(x => x.Reviewer2)
                            .ToListAsync();
        return reviews;
    }

    public async Task<Review?> GetReviewByProjectIdAndNumber(Guid projectId, int number)
    {
        var review = await _dbContext.Reviews.Where(e => e.ProjectId == projectId
                                                        && e.Number == number
                                                        && e.IsDeleted == false)
                                                .SingleOrDefaultAsync();
        return review;
    }

    public async Task<List<Review>?> GetReviewByReviewNumberAndSemesterIdPaging(int number, Guid semesterId)
    {
        var query = await GetQueryable().Where(e =>
                                    e.IsDeleted == false &&
                                    e.Number == number &&
                                    e.Project != null &&
                                    e.Project.Topic.StageTopic.SemesterId == semesterId
                                    )
                                    .Include(x => x.Reviewer1)
                                    .Include(x => x.Reviewer2)
                                    .Include(x => x.Project)
                                    .ThenInclude(x => x.Topic)
                                    .ThenInclude(x => x)
                                    .ToListAsync();

        return query;
    }

    public async Task<List<Review>> GetReviewByReviewerId(Guid reviewerId)
    {
        var queryable = GetQueryable()
            .Include(x => x.Project)
            .ThenInclude(x => x.Topic)
            .ThenInclude(x => x)
            .Where(x => (x.Reviewer1Id == reviewerId || x.Reviewer2Id == reviewerId) && x.ReviewDate.HasValue);

        return await queryable.ToListAsync();
    }
}