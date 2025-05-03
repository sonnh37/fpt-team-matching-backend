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
using FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaVersionRequest;
using FPT.TeamMatching.Domain.Utilities.Filters;
using MongoDB.Driver.Linq;
using NetTopologySuite.Algorithm;

namespace FPT.TeamMatching.Data.Repositories;

public class IdeaRepository : BaseRepository<Idea>, IIdeaRepository
{
    private readonly FPTMatchingDbContext _dbContext;
    private readonly ISemesterRepository _semesterRepository;

    public IdeaRepository(FPTMatchingDbContext dbContext, ISemesterRepository semesterRepository) : base(dbContext)
    {
        _dbContext = dbContext;
        _semesterRepository = semesterRepository;
    }

    public async Task<IList<Idea>> GetIdeasByUserId(Guid userId)
    {
        var ideas = await _dbContext.Ideas.Where(e => e.OwnerId == userId)
            .Include(e => e.IdeaVersions).ThenInclude(e => e.IdeaVersionRequests).ThenInclude(e => e.Reviewer)
            .Include(e => e.IdeaVersions).ThenInclude(e => e.StageIdea)
            .ToListAsync();
        return ideas;
    }

    public async Task<IList<Idea>> GetIdeasByTypeMentorAndEnterprise()
    {
        var ideas = await _dbContext.Ideas.Where(e => e.Type == IdeaType.Enterprise || e.Type == IdeaType.Lecturer)
            .Include(e => e.IdeaVersions).ThenInclude(e => e.IdeaVersionRequests).ThenInclude(e => e.Reviewer)
            .Include(e => e.IdeaVersions).ThenInclude(e => e.StageIdea)
            .ToListAsync();
        return ideas;
    }

    public async Task<List<Idea>> GetCurrentIdeaByUserIdAndStatus(Guid userId, IdeaStatus status)
    {
        var ideas = await _dbContext.Ideas.Where(e => e.OwnerId == userId
                                                      && e.Status == status)
            .OrderByDescending(m => m.CreatedDate)
            .Include(e => e.IdeaVersions).ThenInclude(e => e.IdeaVersionRequests).ThenInclude(e => e.Reviewer)
            .Include(e => e.IdeaVersions).ThenInclude(e => e.StageIdea)
            .Include(e => e.IdeaVersions).ThenInclude(e => e.StageIdea).ThenInclude(m => m.Semester)
            .Include(m => m.Owner)
            .Include(m => m.Mentor)
            .Include(m => m.SubMentor)
            .Include(m => m.Specialty).ThenInclude(m => m.Profession)
            .ToListAsync();

        return ideas;
    }

    public async Task<List<Idea>> GetUserIdeasByStatusWithCurrentStageIdea(Guid? userId, IdeaStatus? status,
        Guid? stageIdeaId)
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
                                                       e.IsDeleted == false &&
                                                       //sua db
                                                       e.IdeaVersions.Any(iv =>
                                                           iv.StageIdea != null &&
                                                           iv.StageIdea.SemesterId == semesterId)).CountAsync();
        return number;
    }

    public async Task<List<Idea>> GetIdeaWithResultDateIsToday()
    {
        var todayLocalMidnight = DateTime.Now.Date; // VD: 24/4/2024 00:00:00 GMT+7

        // Chuyển sang UTC (VD: 23/4/2024 17:00:00 GMT+0 nếu bạn ở GMT+7)
        var todayUtcMidnight = todayLocalMidnight.ToUniversalTime();
        var ideas = await GetQueryable()
            .Include(e => e.Owner).ThenInclude(e => e.UserXRoles).ThenInclude(e => e.Role)
            //sua db
            // .Include(e => e.IdeaVersions).ThenInclude(m => m.StageIdea)
            // .Include(e => e.).ThenInclude(m => m.StageIdea)
            .Where(e =>
                e.IsDeleted == false &&
                e.Status == IdeaStatus.Pending &&
                e.IdeaVersions.Any(iv =>
                    iv.StageIdea != null &&
                    iv.StageIdea.ResultDate.Year == todayUtcMidnight.Year &&
                    iv.StageIdea.ResultDate.Month == todayUtcMidnight.Month &&
                    iv.StageIdea.ResultDate.Day == todayUtcMidnight.Day
                )
            ).ToListAsync();

        return ideas;
    }

    public async Task<(List<Idea>, int)> GetIdeasOfReviewerByRolesAndStatus(
        IdeaGetListByStatusAndRoleQuery query, Guid userId)
    {
        var queryable = GetQueryable()
            .Include(i => i.Owner)
            .Include(i => i.Mentor)
            .Include(i => i.SubMentor)
            .Include(i => i.Specialty)
            .Include(i => i.IdeaVersions)
            .ThenInclude(iv => iv.StageIdea)
            .Include(i => i.IdeaVersions).ThenInclude(m => m.Topic).ThenInclude(m => m.Project)
            .Include(i => i.IdeaVersions)
            .ThenInclude(iv => iv.IdeaVersionRequests)
            .ThenInclude(iv => iv.AnswerCriterias)
            .Where(i => i.IdeaVersions.Any(iv =>
                iv.IdeaVersionRequests.Any(ivr =>
                    ivr.Status != null &&
                    ivr.Role != null &&
                    query.Roles.Contains(ivr.Role) &&
                    query.Status == ivr.Status &&
                    ivr.ReviewerId == userId)));

        // Thêm điều kiện kiểm tra Topic null nếu có role Mentor, 
        // Mentor: thì chỉ lấy những idea chưa có topic
        // Council: lấy idea có topic
        if (query.Roles.Contains("Mentor") || query.Roles.Contains("SubMentor"))
        {
            queryable = queryable.Where(i => i.IdeaVersions.All(iv => iv.Topic == null));
        }

        queryable = queryable.Where(m => m.Status == query.IdeaStatus);

        queryable = Sort(queryable, query);

        var total = await queryable.CountAsync();
        var results = query.IsPagination
            ? await GetQueryablePagination(queryable, query).ToListAsync()
            : await queryable.ToListAsync();

        return (results, query.IsPagination ? total : results.Count);
    }

    public async Task<Idea?> GetIdeaPendingInStageIdeaOfUser(Guid? userId, Guid stageIdeaId)
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

    public async Task<Idea?> GetIdeaApproveInSemesterOfUser(Guid? userId, Guid semesterId)
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
        //return await _dbContext.Ideas.Include(x => x.Project).Where(x => ideaCode.Contains(x.IdeaCode)).ToListAsync();
        return await _dbContext.Ideas
            .Include(x => x.IdeaVersions)
            .ThenInclude(x => x.Topic)
            .ThenInclude(x => x.Project)
            .Where(x => x.IdeaVersions
                .Any(iv => iv.Topic != null && ideaCode.Contains(iv.Topic.TopicCode)))
            .ToListAsync();
    }

    public async Task<(List<Idea>, int)> GetIdeasOfSupervisors(IdeaGetListOfSupervisorsQuery query)
    {
        var semester = await _semesterRepository.GetCurrentSemester();
        if (semester == null)
        {
            return (new List<Idea>(), 0);
        }

        var semesterId = semester.Id;
        var currentDate = DateTime.UtcNow; // Sử dụng UTC để tránh vấn đề múi giờ

        var queryable = GetQueryable();
        queryable = queryable
            .Include(m => m.IdeaVersions)
                .ThenInclude(x => x.Topic)
                .ThenInclude(m => m.MentorTopicRequests)
            .Include(m => m.Owner)
                .ThenInclude(u => u.UserXRoles)
                .ThenInclude(ur => ur.Role)
            .Include(m => m.IdeaVersions)
                .ThenInclude(x => x.Topic)
                .ThenInclude(m => m.Project)
            .Include(m => m.Mentor)
            .Include(m => m.SubMentor)
            .Include(m => m.IdeaVersions)
                .ThenInclude(x => x.StageIdea)
                .ThenInclude(s => s.Semester) // Thêm include cho Semester
            .Include(m => m.Specialty)
                .ThenInclude(m => m.Profession);

        // Thêm điều kiện kiểm tra publicTopicDate
        queryable = queryable.Where(m =>
            m.IdeaVersions.Any(mx =>
                mx.Topic != null &&
                mx.StageIdea != null &&
                mx.StageIdea.Semester != null &&
                mx.StageIdea.SemesterId == semesterId &&
                mx.StageIdea.Semester.PublicTopicDate != null && // Kiểm tra có publicTopicDate
                mx.StageIdea.Semester.PublicTopicDate <= currentDate && // Đã qua ngày công bố
                mx.Topic.MentorTopicRequests.All(x => x.Status != MentorTopicRequestStatus.Approved)));

        // Các điều kiện lọc khác giữ nguyên
        if (!string.IsNullOrEmpty(query.EnglishName))
        {
            queryable = queryable.Where(mi =>
                mi.IdeaVersions.Any(m =>
                    m.EnglishName != null &&
                    m.EnglishName.ToLower().Trim().Contains(query.EnglishName.ToLower().Trim())));
        }

        if (query.IsExistedTeam != null)
        {
            queryable = queryable.Where(m => query.IsExistedTeam.Value == m.IsExistedTeam);
        }

        if (query.Types.Count > 0)
        {
            queryable = queryable.Where(m =>
                m.Type != null && query.Types.Contains(m.Type.Value));
        }

        if (query.Status != null)
        {
            queryable = queryable.Where(m => m.Status == query.Status);
        }

        queryable = BaseFilterHelper.Base(queryable, query);
        queryable = Sort(queryable, query);

        var total = await queryable.CountAsync();
        var results = query.IsPagination
            ? await GetQueryablePagination(queryable, query).ToListAsync()
            : await queryable.ToListAsync();

        return (results, query.IsPagination ? total : results.Count);
    }

    public List<Idea>? GetIdeasOnlyMentorOfUserInSemester(Guid mentorId, Guid semesterId)
    {
        var queryable = GetQueryable();

        var ideas = queryable.Where(e => e.IsDeleted == false &&
                                            e.MentorId == mentorId &&
                                            e.SubMentorId == null &&
                                            e.Status != IdeaStatus.Rejected)
                                .Where(i => i.IdeaVersions != null &&
                                    i.IdeaVersions.OrderByDescending(iv => iv.Version).FirstOrDefault() != null);

        var result = ideas.Where(e => e.IdeaVersions.Any(e => e.StageIdea != null &&
                                                              e.StageIdea.SemesterId == semesterId))
            .ToList();

        return result;
    }

    public List<Idea>? GetIdeasBeSubMentorOfUserInSemester(Guid subMentorId, Guid semesterId)
    {
        var queryable = GetQueryable();

        var ideas = queryable.Where(e => e.IsDeleted == false &&
                                            e.SubMentorId == subMentorId &&
                                            e.Status != IdeaStatus.Rejected)
                                .Where(i => i.IdeaVersions != null &&
                                    i.IdeaVersions.OrderByDescending(iv => iv.Version).FirstOrDefault() != null);

        var result = ideas.Where(e => e.IdeaVersions.Any(e => e.StageIdea != null &&
                                                              e.StageIdea.SemesterId == semesterId))
            .ToList();

        return result;
    }

    public async Task<Idea?> GetIdeaNotRejectOfLeaderInSemester(Guid leaderId, Guid semesterId)
    {
        var queryable = GetQueryable();

        var idea = queryable.Where(e => e.IsDeleted == false &&
                                        e.OwnerId == leaderId &&
                                        e.Status != IdeaStatus.Rejected)
                            .Where(i => i.IdeaVersions != null &&
                                                        i.IdeaVersions.OrderByDescending(iv => iv.Version).FirstOrDefault() != null);

        var result = await idea.Where(e => e.IdeaVersions.Any(e => e.StageIdea != null &&
                                                                    e.StageIdea.SemesterId == semesterId))
                                .Include(i => i.IdeaVersions)
                                .SingleOrDefaultAsync();

        return result;
    }

    public async Task<List<Idea>?> GetIdeaNotApproveInSemester(Guid semesterId)
    {
        var queryable = GetQueryable();

        var ideas = queryable.Where(e => e.IsDeleted == false &&
                                        e.Status != IdeaStatus.Approved)
                            .Where(e => e.IdeaVersions != null &&
                                        e.IdeaVersions.OrderByDescending(iv => iv.Version).FirstOrDefault() != null);

        var result = await ideas.Where(e => e.IdeaVersions.Any(e => e.StageIdea != null &&
                                                                    e.StageIdea.SemesterId == semesterId))
                    .ToListAsync();

        return result;
    }
}