using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Reviews;

namespace FPT.TeamMatching.Data.Repositories;

public class IdeaRepository : BaseRepository<Idea>, IIdeaRepository
{
    private readonly FPTMatchingDbContext _dbContext;

    public IdeaRepository(FPTMatchingDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IList<Idea>> GetIdeasByUserId(Guid userId)
    {
        var ideas = await _dbContext.Ideas.Where(e => e.OwnerId == userId)
            .Include(e => e.IdeaRequests).ThenInclude(e => e.Reviewer)
            .Include(e => e.StageIdea)
            .ToListAsync();
        return ideas;
    }

    public async Task<IList<Idea>> GetIdeasByTypeMentorAndEnterprise()
    {
        var ideas = await _dbContext.Ideas.Where(e => e.Type == IdeaType.Enterprise || e.Type == IdeaType.Lecturer)
            .Include(e => e.IdeaRequests).ThenInclude(e => e.Reviewer)
            .Include(e => e.StageIdea)
            .ToListAsync();
        return ideas;
    }

    public async Task<Idea?> GetLatestIdeaByUserAndStatus(Guid userId, IdeaStatus status)
    {
        return await GetQueryable<Idea>()
            .OrderByDescending(m => m.CreatedDate)
            .FirstOrDefaultAsync(e => e.OwnerId == userId && e.Status == status);
    }

    public async Task<List<Idea>> GetCurrentIdeaByUserIdAndStatus(Guid userId, IdeaStatus status)
    {
        var ideas = await _dbContext.Ideas.Where(e => e.OwnerId == userId
                                                      && e.Status == status)
            .OrderByDescending(m => m.CreatedDate)
            .Include(m => m.StageIdea)
            .Include(e => e.IdeaRequests).ThenInclude(e => e.Reviewer)
            .ToListAsync();

        return ideas;
    }

    public async Task<int> NumberApprovedIdeasOfSemester(Guid? semesterId)
    {
        var number = await _dbContext.Ideas.Where(e => e.Status == IdeaStatus.Approved &&
                                                          e.IsDeleted == false &&
                                                          e.StageIdea != null &&
                                                          e.StageIdea.SemesterId == semesterId).CountAsync();
        return number;
    }

    public async Task<List<Idea>> GetIdeaWithResultDateIsToday()
    {
        var toDay = DateTime.Now.Date;
        var ideas = await _dbContext.Ideas
            .Include(e => e.Owner).ThenInclude(e => e.UserXRoles).ThenInclude(e => e.Role)
            .Include(e => e.StageIdea)
            .Where(e =>
                e.IsDeleted == false &&
                e.Status == IdeaStatus.Pending &&
                e.StageIdea != null &&
                e.StageIdea.ResultDate.LocalDateTime.Date == toDay)
            .ToListAsync();

        return ideas;
    }

    public async Task<Idea?> GetIdeaPendingInStageIdeaOfUser(Guid userId, Guid stageIdeaId)
    {
        var i = await _dbContext.Ideas.Where(e => e.IsDeleted == false &&
                                                  e.StageIdeaId == stageIdeaId &&
                                                  e.OwnerId == userId &&
                                                  e.Status == IdeaStatus.Pending)
            .FirstOrDefaultAsync();
        return i;
    }

    public async Task<Idea?> GetIdeaApproveInSemesterOfUser(Guid userId, Guid semesterId)
    {
        var i = await _dbContext.Ideas.Where(e => e.IsDeleted == false &&
                                                  e.StageIdea != null &&
                                                  e.StageIdea.Semester != null &&
                                                  e.StageIdea.Semester.Id == semesterId &&
                                                  e.OwnerId == userId &&
                                                  e.Status == IdeaStatus.Approved)
            .FirstOrDefaultAsync();
        return i;
    }

    public async Task<int> NumberOfIdeaMentorOrOwner(Guid userId)
    {
        var number = await _dbContext.Ideas.Where(e => e.IsDeleted == false &&
                                                       (e.MentorId == userId || e.OwnerId == userId))
            .CountAsync();
        return number;
    }

    public async Task<List<CustomIdeaResultModel>> GetCustomIdea(Guid semesterId, int reviewNumber)
    {
        // Use async query execution and optimize the LINQ query
        return await GetQueryable()
            .Where(x => x.StageIdea.SemesterId == semesterId)
            .Where(x => x.Project.Reviews.Any(review => review.Number == reviewNumber))
            .Select(y => new CustomIdeaResultModel
            {
                IdeaId = y.Id,
                TeamCode = y.Project.TeamCode,
                IdeaCode = y.IdeaCode,
                Review = y.Project.Reviews
                    .Where(review => review.Number == reviewNumber)
                    .Select(review => new ReviewUpdateCommand
                    {
                        Id = review.Id,
                        Number = review.Number,
                        Description = review.Description,
                        Reviewer1 = review.Reviewer1Id,
                        Reviewer2 = review.Reviewer2Id,
                        FileUpload = review.FileUpload,
                        ProjectId = review.ProjectId,
                    })
                    .FirstOrDefault()
            })
            .ToListAsync();
    }
}