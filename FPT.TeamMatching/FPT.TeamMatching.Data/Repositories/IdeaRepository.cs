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
using FPT.TeamMatching.Domain.Models.Requests.Queries.Ideas;

namespace FPT.TeamMatching.Data.Repositories;

public class IdeaRepository : BaseRepository<Idea>, IIdeaRepository
{
    private readonly FPTMatchingDbContext _dbContext;
    private readonly IStageIdeaRepositoty _stageIdeaRepositoty;

    public IdeaRepository(FPTMatchingDbContext dbContext, IStageIdeaRepositoty stageIdeaRepositoty) : base(dbContext)
    {
        _dbContext = dbContext;
        _stageIdeaRepositoty = stageIdeaRepositoty;
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
            .Include(m => m.Owner)
            .Include(m => m.Mentor)
            .Include(m => m.SubMentor)
            .Include(m => m.Specialty)
            .Include(m => m.Project)
            .Include(e => e.IdeaRequests).ThenInclude(e => e.Reviewer)
            .ToListAsync();

        return ideas;
    }
    
    public async Task<List<Idea>> GetUserIdeasByStatusWithCurrentStageIdea(Guid? userId, IdeaStatus? status, Guid? stageIdeaId)
    {
        var ideas = await _dbContext.Ideas.Where(e => e.OwnerId == userId
                                                      && e.Status == status
                                                      && e.StageIdeaId == stageIdeaId
                                                      )
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
        var toDay = DateTime.UtcNow;
        var ideas = await _dbContext.Ideas
            .Include(e => e.Owner).ThenInclude(e => e.UserXRoles).ThenInclude(e => e.Role)
            .Include(e => e.StageIdea)
            .Where(e =>
                e.IsDeleted == false &&
                e.Status == IdeaStatus.Pending &&
                e.StageIdea != null &&
                (e.StageIdea.ResultDate.Year == toDay.Year &&
                 e.StageIdea.ResultDate.Month == toDay.Month &&
                 e.StageIdea.ResultDate.Day == toDay.Day))
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
                        Reviewer1Id = review.Reviewer1Id,
                        Reviewer2Id = review.Reviewer2Id,
                        FileUpload = review.FileUpload,
                        ProjectId = review.ProjectId,
                    })
                    .FirstOrDefault()
            })
            .ToListAsync();
    }

    public async Task<List<Idea>> GetIdeasByIdeaCodes(string[] ideaCode)
    {
        return await _dbContext.Ideas.Include(x => x.Project).Where(x => ideaCode.Contains(x.IdeaCode)).ToListAsync();
    }

    public async Task<(List<Idea>, int)> GetIdeasOfSupervisors(IdeaGetListOfSupervisorsQuery query)
    {
        var queryable = GetQueryable();
        queryable = queryable
            .Include(m => m.IdeaRequests)
            .Include(m => m.Owner)
            .ThenInclude(u => u.UserXRoles)
            .ThenInclude(ur => ur.Role)
            .Include(m => m.Project)
            .Include(m => m.Mentor)
            .Include(m => m.SubMentor)
            .Include(m => m.StageIdea)
            .Include(m => m.MentorIdeaRequests)
            .Include(m => m.Specialty).ThenInclude(m => m.Profession);
        // 
        // queryable = queryable.Where(m => m.MentorIdeaRequests.All(x => x.Status != MentorIdeaRequestStatus.Approved));

        if (!string.IsNullOrEmpty(query.EnglishName))
        {
            queryable = queryable.Where(m =>
                m.EnglishName != null && m.EnglishName.ToLower().Trim().Contains(query.EnglishName.ToLower().Trim()));
        }
        
        if (query.IsExistedTeam != null) queryable = queryable.Where(m => query.IsExistedTeam.Value == m.IsExistedTeam);

        if (query.Types.Count > 0)
        {
            queryable = queryable.Where(m =>
                m.Type != null && query.Types.Contains(m.Type.Value));
        }

        if (query.Status != null)
        {
            queryable = queryable.Where(m => m.Status == query.Status);
        }

        //
        if (query.IsPagination)
        {
            var totalOrigin = queryable.Count();
            queryable = Sort(queryable, query);
            var results = await GetQueryablePagination(queryable, query).ToListAsync();

            return (results, totalOrigin);
        }
        else
        {
            queryable = Sort(queryable, query);
            var results = await queryable.ToListAsync();
            return (results, results.Count);
        }
    }
}