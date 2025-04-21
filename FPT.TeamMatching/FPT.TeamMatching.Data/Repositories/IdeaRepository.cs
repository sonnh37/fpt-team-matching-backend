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
            .Include(e => e.IdeaVersions).ThenInclude(e => e.IdeaVersionRequests).ThenInclude(e => e.Reviewer)
            .Include(e => e.IdeaVersions).ThenInclude(e => e.StageIdea)
            //.Include(e => e.IdeaVersionRequests).ThenInclude(e => e.Reviewer)
            //.Include(e => e.StageIdea)
            .ToListAsync();
        return ideas;
    }

    public async Task<IList<Idea>> GetIdeasByTypeMentorAndEnterprise()
    {
        var ideas = await _dbContext.Ideas.Where(e => e.Type == IdeaType.Enterprise || e.Type == IdeaType.Lecturer)
            .Include(e => e.IdeaVersions).ThenInclude(e => e.IdeaVersionRequests).ThenInclude(e => e.Reviewer)
            .Include(e => e.IdeaVersions).ThenInclude(e => e.StageIdea)
            //.Include(e => e.IdeaVersionRequests).ThenInclude(e => e.Reviewer)
            //.Include(e => e.StageIdea)
            .ToListAsync();
        return ideas;
    }
    
    public async Task<List<Idea>> GetCurrentIdeaByUserIdAndStatus(Guid userId, IdeaStatus status)
    {
        var ideas = await _dbContext.Ideas.Where(e => e.OwnerId == userId
                                                      && e.Status == status)
            .OrderByDescending(m => m.CreatedDate)
            //sua db
            //.Include(m => m.StageIdea)
            .Include(e => e.IdeaVersions).ThenInclude(e => e.IdeaVersionRequests).ThenInclude(e => e.Reviewer)
            .Include(e => e.IdeaVersions).ThenInclude(e => e.StageIdea)
            .Include(e => e.IdeaVersions).ThenInclude(e => e.StageIdea).ThenInclude(m => m.Semester)

            .Include(m => m.Owner)
            .Include(m => m.Mentor)
            .Include(m => m.SubMentor)
            .Include(m => m.Specialty).ThenInclude(m => m.Profession)
            //sua db
            .ToListAsync();

        return ideas;
    }
    
    public async Task<List<Idea>> GetUserIdeasByStatusWithCurrentStageIdea(Guid? userId, IdeaStatus? status, Guid? stageIdeaId)
    {
        var ideas = await _dbContext.Ideas.Where(e => e.OwnerId == userId
                                                      && e.Status == status
                                                      //sua db
                                                      //&& e.StageIdeaId == stageIdeaId
                                                      )
            .OrderByDescending(m => m.CreatedDate)
            //.Include(m => m.StageIdea)
            //.Include(e => e.IdeaVersionRequests).ThenInclude(e => e.Reviewer)
            .ToListAsync();

        return ideas;
    }

    public async Task<int> NumberApprovedIdeasOfSemester(Guid? semesterId)
    {
        var number = await _dbContext.Ideas.Where(e => e.Status == IdeaStatus.Approved &&
                                                       e.IsDeleted == false)
            //sua db
                                                       //&&
                                                       //e.StageIdea != null &&
                                                       //e.StageIdea.SemesterId == semesterId)
                                            .CountAsync();
        return number;
    }

    public async Task<List<Idea>> GetIdeaWithResultDateIsToday()
    {
        var toDay = DateTime.UtcNow;
        var ideas = await _dbContext.Ideas
            .Include(e => e.Owner).ThenInclude(e => e.UserXRoles).ThenInclude(e => e.Role)
            //sua db
            //.Include(e => e.StageIdea)
            //.Where(e =>
            //    e.IsDeleted == false &&
            //    e.Status == IdeaStatus.Pending &&
            //    e.StageIdea != null &&
            //    (e.StageIdea.ResultDate.Year == toDay.Year &&
            //     e.StageIdea.ResultDate.Month == toDay.Month &&
            //     e.StageIdea.ResultDate.Day == toDay.Day))
            .ToListAsync();

        return ideas;
    }

    public async Task<Idea?> GetIdeaPendingInStageIdeaOfUser(Guid userId, Guid stageIdeaId)
    {
        var idea = await _dbContext.Ideas
            .Include(i => i.IdeaVersions)
            .Where(e => e.IsDeleted == false &&
                        e.OwnerId == userId &&
                        e.Status == IdeaStatus.Pending &&
                        e.IdeaVersions.Any(iv => iv.StageIdeaId == stageIdeaId))
            .FirstOrDefaultAsync();
    
        return idea;
    }

    public async Task<Idea?> GetIdeaApproveInSemesterOfUser(Guid userId, Guid semesterId)
    {
        var idea = await _dbContext.Ideas
            .Include(i => i.IdeaVersions)
            .ThenInclude(iv => iv.StageIdea)
            .Where(e => e.IsDeleted == false &&
                        e.OwnerId == userId &&
                        e.Status == IdeaStatus.Approved &&
                        e.IdeaVersions.Any(iv => 
                            iv.StageIdea != null && 
                            iv.StageIdea.SemesterId == semesterId))
            .FirstOrDefaultAsync();
    
        return idea;
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
                .Include(x => x.IdeaVersions)
                .ThenInclude(x => x.StageIdea)
                .Include(x => x.IdeaVersions)
                .ThenInclude(x => x.Topic)
                .ThenInclude(x => x.Project)
                .ThenInclude(p => p.Reviews)
                .Where(x => x.IdeaVersions.Any(iv =>
                    iv.StageIdea.SemesterId == semesterId &&
                    iv.Topic.Project.Reviews.Any(r => r.Number == reviewNumber) 
                ))
            .Select(y => new CustomIdeaResultModel
            {
                IdeaId = y.Id,
                TeamCode = y.IdeaVersions.FirstOrDefault().Topic.Project.TeamCode,
                IdeaCode = y.IdeaVersions.FirstOrDefault().Topic.TopicCode,
                Review = y.IdeaVersions.FirstOrDefault().Topic.Project.Reviews
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
        //sua db
        //return await _dbContext.Ideas.Include(x => x.Project).Where(x => ideaCode.Contains(x.IdeaCode)).ToListAsync();
        return await _dbContext.Ideas.Include(x => x.IdeaVersions).ToListAsync();
    }

    public async Task<(List<Idea>, int)> GetIdeasOfSupervisors(IdeaGetListOfSupervisorsQuery query)
    {
        var queryable = GetQueryable();
        queryable = queryable
            //sua db
            //.Include(m => m.IdeaVersionRequests)
            .Include(m => m.Owner)
            .ThenInclude(u => u.UserXRoles)
            .ThenInclude(ur => ur.Role)
            //sua db
            //.Include(m => m.Project)
            .Include(m => m.Mentor)
            .Include(m => m.SubMentor)
            //.Include(m => m.StageIdea)
            //.Include(m => m.MentorTopicRequests)
            .Include(m => m.Specialty).ThenInclude(m => m.Profession);
        // 
        // queryable = queryable.Where(m => m.MentorTopicRequests.All(x => x.Status != MentorTopicRequestStatus.Approved));

        //sua db
        //if (!string.IsNullOrEmpty(query.EnglishName))
        //{
        //    queryable = queryable.Where(m =>
        //        m.EnglishName != null && m.EnglishName.ToLower().Trim().Contains(query.EnglishName.ToLower().Trim()));
        //}
        
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