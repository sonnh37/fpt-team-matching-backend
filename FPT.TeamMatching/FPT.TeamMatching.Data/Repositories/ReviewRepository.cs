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
                                                        //sua db
                                                        //&& e.Number == number
                                                        && e.IsDeleted == false)
                                                .SingleOrDefaultAsync();
        return review;
    }

    public async Task<List<Review>?> GetReviewByReviewNumberAndSemesterIdPaging(int number, Guid semesterId)
    {
        var query = await _dbContext.Reviews.Where(e =>
                                    e.IsDeleted == false &&
                                    e.Number == number &&
                                    e.Project != null 
                                    //sua db
                                    //&&
                                    //e.Project.Idea != null &&
                                    //e.Project.Idea.StageIdea != null &&
                                    //e.Project.Idea.StageIdea.SemesterId == semesterId
                                    )
                                    //.Include(e => e.Project).ThenInclude(e => e.Idea)
                                    .ToListAsync();

        return query;
    }
}