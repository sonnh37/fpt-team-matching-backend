using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

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
                                        .ToListAsync();
        return ideas;
    }

    public async Task<List<Idea>> GetCurrentIdeaByUserIdAndStatus(Guid userId, IdeaStatus status)
    {
        var ideas = await _dbContext.Ideas.Where(e => e.OwnerId == userId
             && e.Status == status)
            .OrderByDescending(m => m.CreatedDate)
            .Include(e => e.IdeaRequests).ThenInclude(e => e.Reviewer)
            .ToListAsync();

        return ideas;
    }

    public async Task<int> NumberApprovedIdeasOfSemester(Guid? semesterId)
    {
        var maxNumber = await _dbContext.Ideas.Where(e => e.Status == IdeaStatus.Approved && 
                                                        e.IsDeleted == false &&
                                                        e.StageIdea != null &&
                                                        e.StageIdea.SemesterId == semesterId).CountAsync();
        return maxNumber;
    }

    public async Task<List<Idea>> GetIdeaWithResultDateIsToday()
    {
        var ideas = await _dbContext.Ideas
            .Where(e =>
                                                    e.IsDeleted == false &&
                                                    e.Status == IdeaStatus.Pending &&
                                                    e.StageIdea != null &&
                                                    e.StageIdea.ResultDate.UtcDateTime.Date == DateTime.UtcNow.Date)
                                            .Include(e => e.StageIdea).ThenInclude(e => e.Semester)
                                            .Include(e => e.Owner).ThenInclude(e => e.UserXRoles).ThenInclude(e => e.Role)
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
                                                (e.MentorId ==  userId || e.OwnerId == userId))
                                        .CountAsync();
        return number;
    }
}