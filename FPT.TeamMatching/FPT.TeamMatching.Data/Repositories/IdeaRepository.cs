using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Reviews;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Ideas;
using FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaVersionRequest;
using FPT.TeamMatching.Domain.Utilities.Filters;
using MongoDB.Driver.Linq;

namespace FPT.TeamMatching.Data.Repositories;

public class IdeaRepository : BaseRepository<Idea>, IIdeaRepository
{
    private readonly ISemesterRepository _semesterRepository;
    private readonly FPTMatchingDbContext _dbContext;

    public IdeaRepository(FPTMatchingDbContext dbContext, ISemesterRepository semesterRepository) : base(dbContext)
    {
        _semesterRepository = semesterRepository;
        _dbContext = dbContext;
    }

    public async Task<IList<Idea>> GetIdeasByUserId(Guid userId)
    {
        var queryable = GetQueryable();

        var ideas = await queryable.Where(e => e.OwnerId == userId)
            .Include(e => e.IdeaVersions).ThenInclude(e => e.IdeaVersionRequests).ThenInclude(e => e.Reviewer)
            .Include(e => e.IdeaVersions).ThenInclude(e => e.StageIdea)
            .ToListAsync();
        return ideas;
    }

    public async Task<IList<Idea>> GetIdeasByTypeMentorAndEnterprise()
    {
        var queryable = GetQueryable();

        var ideas = await queryable.Where(e => e.Type == IdeaType.Enterprise || e.Type == IdeaType.Lecturer)
            .Include(e => e.IdeaVersions).ThenInclude(e => e.IdeaVersionRequests).ThenInclude(e => e.Reviewer)
            .Include(e => e.IdeaVersions).ThenInclude(e => e.StageIdea)
            .ToListAsync();
        return ideas;
    }

    public async Task<List<Idea>> GetCurrentIdeaByUserIdAndStatus(Guid userId, IdeaStatus status)
    {
        var queryable = GetQueryable();

        var ideas = await queryable.Where(e => e.OwnerId == userId
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
        var queryable = GetQueryable();

        var ideas = await queryable.Where(e => e.OwnerId == userId
                                               && e.Status == status
            )
            .OrderByDescending(m => m.CreatedDate)
            .ToListAsync();

        return ideas;
    }

    public async Task<int> NumberApprovedIdeasOfSemester(Guid? semesterId)
    {
        var queryable = GetQueryable();

        var number = await queryable.Where(e => e.Status == IdeaStatus.Approved &&
                                                e.IsDeleted == false &&
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
        var semester = await _semesterRepository.GetUpComingSemester();
        if (semester == null)
        {
            return (new List<Idea>(), 0);
        }

        var queryable = GetQueryable()
            .Include(i => i.Owner).ThenInclude(m => m.Projects)
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
                iv.Version == i.IdeaVersions.Max(iv2 => iv2.Version) &&
                iv.IdeaVersionRequests.Any(ivr =>
                    ivr.Status != null &&
                    ivr.Role != null &&
                    query.Roles.Contains(ivr.Role) &&
                    query.Status == ivr.Status &&
                    ivr.ReviewerId == userId)));

        queryable = queryable.Where((m =>
            m.IdeaVersions.Any(i => i.StageIdea != null && i.StageIdea.SemesterId == semester.Id)));
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
        var queryable = GetQueryable();

        var idea = await queryable
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
        var queryable = GetQueryable();

        var idea = await queryable
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
        var queryable = GetQueryable();

        var number = await queryable.Where(e => e.IsDeleted == false &&
                                                (e.MentorId == userId || e.OwnerId == userId))
            .CountAsync();
        return number;
    }

    public async Task<List<CustomIdeaResultModel>> GetCustomIdea(Guid semesterId, int reviewNumber)
    {
        // Use async query execution and optimize the LINQ query
        var query = await _dbContext.Ideas
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
            
            .ToListAsync();
            var result = query.Select(y => new CustomIdeaResultModel
            {
                IdeaId = y.Id,
                TeamCode = y.IdeaVersions.FirstOrDefault(x => x.Version == y.IdeaVersions.Max(x => x.Version)).Topic.Project.TeamCode,
                IdeaCode = y.IdeaVersions.FirstOrDefault(x => x.Version == y.IdeaVersions.Max(x => x.Version)).Topic.TopicCode,
                Review = y.IdeaVersions.FirstOrDefault(x => x.Version == y.IdeaVersions.Max(x => x.Version)).Topic.Project.Reviews
                    .Where(review => review.Number == reviewNumber)
                    .Select(review => new ReviewUpdateCommand
                    {
                        Id = review.Id,
                        Number = review.Number,
                        Description = review.Description,
                        Reviewer1Id = review.Reviewer1Id,
                        Reviewer2Id = review.Reviewer2Id,
                        FileUpload = review.FileUpload,
                        ProjectId = review.ProjectId.Value,
                    })
                    .FirstOrDefault()
            }).ToList();
        return result;
    }

    public async Task<List<Idea>> GetIdeasByIdeaCodes(string[] ideaCode)
    {
        var queryable = GetQueryable();

        return await queryable
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

    public async Task<Idea?> GetIdeaNotRejectOfUserInSemester(Guid userId, Guid semesterId)
    {
        var queryable = GetQueryable();

        var idea = queryable.Include(m => m.IdeaVersions).Where(e => e.IsDeleted == false &&
                                                                     e.OwnerId == userId &&
                                                                     e.Status != IdeaStatus.Rejected)
            .Where(i => i.IdeaVersions.OrderByDescending(iv => iv.Version).FirstOrDefault() != null);

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

    public async Task<Idea> GetIdeaByProjectId(Guid projectId)
    {
        var queryable = await GetQueryable().Include(x => x.IdeaVersions)
            .ThenInclude(x => x.Topic)
            .ThenInclude(x => x.Project)
            .FirstOrDefaultAsync(x => x.IdeaVersions.FirstOrDefault(y => y.Version == x.IdeaVersions.Max(z => z.Version)).Topic.Project.Id == projectId);
        return queryable;
    }
}