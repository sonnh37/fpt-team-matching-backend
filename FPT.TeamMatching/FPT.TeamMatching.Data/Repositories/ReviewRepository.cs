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

    public async Task<(List<Review>?, int)> GetReviewByReviewNumberAndSemesterIdPaging(int number, Guid semesterId, int pageIndex, int pageSize)
    {
        var query = _dbContext.Reviews.Where(e =>
                                    e.IsDeleted == false &&
                                    e.Number == number &&
                                    e.Project != null &&
                                    e.Project.Idea != null &&
                                    e.Project.Idea.StageIdea != null &&
                                    e.Project.Idea.StageIdea.SemesterId == semesterId);

        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalRecords = items.Count();

        return (items, totalRecords);
    }
}