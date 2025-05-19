using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Reviews;
using FPT.TeamMatching.Domain.Utilities.Filters;
using MongoDB.Driver.Linq;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Topics;
using FPT.TeamMatching.Domain.Models.Requests.Queries.TopicRequest;

namespace FPT.TeamMatching.Data.Repositories;

public class TopicRepository : BaseRepository<Topic>, ITopicRepository
{
    private readonly ISemesterRepository _semesterRepository;
    private readonly FPTMatchingDbContext _dbContext;

    public TopicRepository(FPTMatchingDbContext dbContext, ISemesterRepository semesterRepository) : base(dbContext)
    {
        _semesterRepository = semesterRepository;
        _dbContext = dbContext;
    }

    public async Task<IList<Topic>> GetTopicsByUserId(Guid userId)
    {
        var queryable = GetQueryable();

        var ideas = await queryable.Where(e => e.OwnerId == userId)
            .Include(e => e.TopicVersions).ThenInclude(e => e.TopicVersionRequests).ThenInclude(e => e.Reviewer)
            .Include(e => e.StageTopic)
            .ToListAsync();
        return ideas;
    }

    public async Task<IList<Topic>> GetTopicsByTypeMentorAndEnterprise()
    {
        var queryable = GetQueryable();

        var ideas = await queryable.Where(e => e.Type == TopicType.Enterprise || e.Type == TopicType.Lecturer)
            .Include(e => e.TopicVersions).ThenInclude(e => e.TopicVersionRequests).ThenInclude(e => e.Reviewer)
            .Include(e => e.StageTopic)
            .ToListAsync();
        return ideas;
    }

    public async Task<List<Topic>> GetCurrentTopicByUserIdAndStatus(Guid? userId, Guid? semesterId, List<TopicStatus> statusList)
    {
        var queryable = GetQueryable();

        var ideas = await queryable.Where(e => e.OwnerId == userId
                                               && e.SemesterId == semesterId
                                               && e.Status != null
                                               && statusList.Contains(e.Status.Value))
            .OrderByDescending(m => m.CreatedDate)
            .Include(e => e.TopicVersions).ThenInclude(e => e.TopicVersionRequests).ThenInclude(e => e.Reviewer)
            .Include(e => e.StageTopic)
            .Include(m => m.Owner)
            .Include(m => m.Mentor)
            .Include(m => m.Semester)
            .Include(m => m.SubMentor)
            .Include(m => m.Specialty).ThenInclude(m => m.Profession)
            .ToListAsync();

        return ideas;
    }

    public async Task<List<Topic>> GetUserTopicsByStatusWithCurrentStageTopic(Guid? userId, TopicStatus? status,
        Guid? stageTopicId)
    {
        var queryable = GetQueryable();

        var ideas = await queryable.Where(e => e.OwnerId == userId
                                               && e.Status == status
            )
            .OrderByDescending(m => m.CreatedDate)
            .ToListAsync();

        return ideas;
    }

    public async Task<int> NumberApprovedTopicsOfSemester(Guid? semesterId)
    {
        var queryable = GetQueryable();

        var number = await queryable.Where(e => e.Status == TopicStatus.ManagerApproved &&
                                                e.IsDeleted == false &&
                                                e.StageTopic != null &&
                                                e.StageTopic.SemesterId == semesterId)
                                    .CountAsync();
        return number;
    }

    public async Task<List<Topic>> GetTopicWithResultDateIsToday()
    {
        var todayLocalMidnight = DateTime.Now.Date; // VD: 24/4/2024 00:00:00 GMT+7

        // Chuyển sang UTC (VD: 23/4/2024 17:00:00 GMT+0 nếu bạn ở GMT+7)
        var todayUtcMidnight = todayLocalMidnight.ToUniversalTime();
        var ideas = await GetQueryable()
            .Where(e =>
                e.IsDeleted == false &&
                e.Status == TopicStatus.MentorPending &&
                e.StageTopic != null &&
                e.StageTopic.ResultDate.Year == todayUtcMidnight.Year &&
                e.StageTopic.ResultDate.Month == todayUtcMidnight.Month &&
                e.StageTopic.ResultDate.Day == todayUtcMidnight.Day
            ).ToListAsync();

        return ideas;
    }

    public async Task<(List<Topic>, int)> GetTopicsOfReviewerByRolesAndStatus(
        TopicRequestGetListByStatusAndRoleQuery query, Guid userId)
    {
        var semester = await _semesterRepository.GetUpComingSemester();
        if (semester == null)
        {
            return (new List<Topic>(), 0);
        }

        var queryable = GetQueryable()
            .Include(i => i.Owner).ThenInclude(m => m.Projects)
            .Include(i => i.Mentor)
            .Include(i => i.SubMentor)
            .Include(i => i.Specialty)
            .Include(i => i.TopicRequests)
  .Include(i => i.Project) // nếu bạn cần dùng `i.Project` (trước đây là qua Topic → TopicVersion → Topic)
  .Where(i => i.TopicRequests.Any(ivr =>
      ivr.Status != null &&
       ivr.Status == query.Status
      &&
      ivr.Role != null &&
      query.Roles.Contains(ivr.Role) &&
      ivr.ReviewerId == userId
  ));

        queryable = queryable.Where((i => i.StageTopic != null && i.StageTopic.SemesterId == semester.Id));
        // Thêm điều kiện kiểm tra Topic null nếu có role Mentor, 
        // Mentor: thì chỉ lấy những idea chưa có topic
        // Council: lấy idea có topic
       /* if (query.Roles.Contains("Mentor") || query.Roles.Contains("SubMentor"))
        {
            queryable = queryable.Where(i => i.TopicVersions.All(iv => iv.Topic == null));
        }*/

        queryable = queryable.Where(m => m.Status == query.TopicStatus);

        queryable = Sort(queryable, query);

        var total = await queryable.CountAsync();
        var results = query.IsPagination
            ? await GetQueryablePagination(queryable, query).ToListAsync()
            : await queryable.ToListAsync();

        return (results, query.IsPagination ? total : results.Count);
    }

    public async Task<Topic?> GetTopicPendingInStageTopicOfUser(Guid? userId, Guid stageTopicId)
    {
        var queryable = GetQueryable();

        var idea = await queryable
            .Include(i => i.TopicVersions)
            .Where(e => e.IsDeleted == false &&
                        e.OwnerId == userId &&
                        e.Status == TopicStatus.MentorPending &&
                        e.Status == TopicStatus.ManagerPending &&
                        e.StageTopicId == stageTopicId)
            .FirstOrDefaultAsync();

        return idea;
    }

    public async Task<Topic?> GetTopicApproveInSemesterOfUser(Guid? userId, Guid semesterId)
    {
        var queryable = GetQueryable();

        var idea = await queryable
            .Include(i => i.TopicVersions)
            .Include(iv => iv.StageTopic)
            .Where(e => e.IsDeleted == false &&
                        e.OwnerId == userId &&
                        e.Status == TopicStatus.ManagerApproved &&
                        e.StageTopic != null &&
                        e.StageTopic.SemesterId == semesterId)
            .FirstOrDefaultAsync();

        return idea;
    }

    public async Task<int> NumberOfTopicMentorOrOwner(Guid userId)
    {
        var queryable = GetQueryable();

        var number = await queryable.Where(e => e.IsDeleted == false &&
                                                (e.MentorId == userId || e.OwnerId == userId))
            .CountAsync();
        return number;
    }

    public async Task<List<CustomTopicResultModel>> GetCustomTopic(Guid semesterId, int reviewNumber)
    {
        // Use async query execution and optimize the LINQ query
        var query = await _dbContext.Topics
            .Include(x => x.TopicVersions)
            .Include(x => x.StageTopic)
            .Include(x => x.TopicVersions)
            .ThenInclude(x => x.Topic)
            .ThenInclude(x => x.Project)
            .ThenInclude(p => p.Reviews)
            .Where(iv =>
                iv.StageTopic != null &&
                iv.StageTopic.SemesterId == semesterId &&
                iv.Project != null &&
                iv.Project.Reviews.Any(r => r.Number == reviewNumber)
            )
            .ToListAsync();
        var result = query.Select(y => new CustomTopicResultModel
        {
            TopicId = y.Id,
            TeamCode = y.TopicVersions.FirstOrDefault().Topic.Project.TeamCode,
            TopicCode = y.TopicVersions.FirstOrDefault().Topic.TopicCode,
            Review = y.TopicVersions.FirstOrDefault().Topic.Project.Reviews
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

    public async Task<List<Topic>> GetTopicsByTopicCodes(string[] ideaCode)
    {
        var queryable = GetQueryable();

        return await queryable
            .Include(x => x.TopicVersions)
            .ThenInclude(x => x.Topic)
            .ThenInclude(x => x.Project)
            .Where(x => x.TopicVersions
                .Any(iv => iv.Topic != null && ideaCode.Contains(iv.Topic.TopicCode)))
            .ToListAsync();
    }

    public async Task<(List<Topic>, int)> GetTopicsOfSupervisors(TopicGetListOfSupervisorsQuery query)
    {
        var semester = await _semesterRepository.GetCurrentSemester();
        if (semester == null)
        {
            return (new List<Topic>(), 0);
        }

        var semesterId = semester.Id;
        var currentDate = DateTime.UtcNow; // Sử dụng UTC để tránh vấn đề múi giờ

        var queryable = GetQueryable();
        queryable = queryable
            .Include(m => m.TopicVersions)
            .ThenInclude(x => x.Topic)
            .ThenInclude(m => m.MentorTopicRequests)
            .Include(m => m.Owner)
            .ThenInclude(u => u.UserXRoles)
            .ThenInclude(ur => ur.Role)
            .Include(m => m.TopicVersions)
            .ThenInclude(x => x.Topic)
            .ThenInclude(m => m.Project)
            .Include(m => m.Mentor)
            .Include(m => m.SubMentor)
            .Include(m => m.TopicVersions)
            .Include(x => x.StageTopic)
            .ThenInclude(s => s.Semester) 
            .Include(m => m.Specialty)
            .ThenInclude(m => m.Profession);

        // Thêm điều kiện kiểm tra publicTopicDate
        queryable = queryable.Where(mx =>
                mx.StageTopic != null &&
                mx.StageTopic.Semester != null &&
                mx.StageTopic.SemesterId == semesterId &&
                mx.StageTopic.Semester.PublicTopicDate != null && // Kiểm tra có publicTopicDate
                mx.StageTopic.Semester.PublicTopicDate <= currentDate && // Đã qua ngày công bố
                mx.MentorTopicRequests.All(x => x.Status != MentorTopicRequestStatus.Approved));

        // Các điều kiện lọc khác giữ nguyên
        if (!string.IsNullOrEmpty(query.EnglishName))
        {
            queryable = queryable.Where(m =>
                    m.EnglishName != null &&
                    m.EnglishName.ToLower().Trim().Contains(query.EnglishName.ToLower().Trim()));
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

    public async Task<List<Topic>> GetTopicsOnlyMentorOfUserInSemester(Guid mentorId, Guid semesterId)
    {
        var queryable = GetQueryable();

        var ideas = queryable.Where(e => e.IsDeleted == false &&
                                         e.MentorId == mentorId &&
                                         e.SubMentorId == null &&
                                         e.Status != TopicStatus.ManagerRejected)
            .Where(i => i.TopicVersions != null &&
                        i.TopicVersions.OrderByDescending(iv => iv.CreatedDate).FirstOrDefault() != null);

        var result = await ideas.Where(e => e.StageTopic != null &&
                                    e.StageTopic.SemesterId == semesterId)
                                .ToListAsync();

        return result;
    }

    public List<Topic>? GetTopicsBeSubMentorOfUserInSemester(Guid subMentorId, Guid semesterId)
    {
        var queryable = GetQueryable();

        var ideas = queryable.Where(e => e.IsDeleted == false &&
                                         e.SubMentorId == subMentorId &&
                                         e.Status != TopicStatus.ManagerRejected)
            .Where(i => i.TopicVersions != null &&
                        i.TopicVersions.OrderByDescending(iv => iv.CreatedDate).FirstOrDefault() != null);

        var result = ideas.Where(e => e.StageTopic != null &&
                                    e.StageTopic.SemesterId == semesterId)
            .ToList();

        return result;
    }

    public async Task<Topic?> GetTopicNotRejectOfUserInSemester(Guid userId, Guid semesterId)
    {
        var queryable = GetQueryable();

        var idea = queryable.Include(m => m.TopicVersions).Where(e => e.IsDeleted == false &&
                                                                     e.OwnerId == userId &&
                                                                     e.Status != TopicStatus.ManagerRejected)
            .Where(i => i.TopicVersions.OrderByDescending(iv => iv.CreatedDate).FirstOrDefault() != null);

        var result = await idea.Where(e => e.StageTopic != null &&
                                        e.StageTopic.SemesterId == semesterId)
            .Include(i => i.TopicVersions)
            .SingleOrDefaultAsync();

        return result;
    }

    public async Task<List<Topic>?> GetTopicNotApproveInSemester(Guid semesterId)
    {
        var queryable = GetQueryable();

        var ideas = queryable.Where(e => e.IsDeleted == false &&
                                         e.Status != TopicStatus.ManagerApproved)
            .Where(e => e.TopicVersions != null &&
                        e.TopicVersions.OrderByDescending(iv => iv.CreatedDate).FirstOrDefault() != null);

        var result = await ideas.Where(e => e.StageTopic != null &&
                                            e.StageTopic.SemesterId == semesterId)
            .ToListAsync();

        return result;
    }

    public async Task<Topic> GetTopicByProjectId(Guid projectId)
    {
        var queryable = await GetQueryable().Include(x => x.TopicVersions)
            .ThenInclude(x => x.Topic)
            .ThenInclude(x => x.Project)
            .FirstOrDefaultAsync(x => x.TopicVersions.FirstOrDefault().Topic.Project.Id == projectId);
        return queryable;
    }

    public async Task<(List<Topic>, int)> GetTopicsForMentor(TopicGetListForMentorQuery query, Guid userId)
    {
        var queryable = GetQueryable();
        queryable = queryable
            .Include(m => m.TopicRequests)
            .Include(m => m.Project)
            .Include(m => m.MentorTopicRequests)
            .Include(m => m.TopicVersions);

        queryable = queryable.Where(m => m.Status != TopicStatus.ManagerRejected);

        if (query.Roles.Contains("Mentor") && query.Roles.Contains("SubMentor"))
        {
            queryable = queryable.Where(m =>
                (m.MentorId == userId ||
                 m.SubMentorId == userId));
        }
        else if (query.Roles.Contains("Mentor"))
        {
            queryable = queryable.Where(m =>
                m.MentorId == userId);
        }
        else if (query.Roles.Contains("SubMentor"))
        {
            queryable = queryable.Where(m =>
                m.SubMentorId == userId);
        }
        else
        {
            queryable = queryable.Where(m => m.MentorId == userId || m.SubMentorId == userId);
        }

        queryable = BaseFilterHelper.Base(queryable, query);

        queryable = Sort(queryable, query);

        var total = queryable.Count();
        var results = query.IsPagination
            ? await GetQueryablePagination(queryable, query).ToListAsync()
            : await queryable.ToListAsync();

        return (results, query.IsPagination ? total : results.Count);
    }

    public async Task<List<Topic>> ApprovedTopicsBySemesterId(Guid semesterId)
    {
        var queryable = GetQueryable();
        var topics = await queryable.Where(e => e.IsDeleted == false &&
                            e.StageTopic != null &&
                            e.StageTopic.Semester != null &&
                            e.StageTopic.Semester.Id == semesterId)
                            .ToListAsync();
        return topics;
    }

    public async Task<Topic?> GetTopicWithStatusInSemesterOfUser(Guid userId, Guid semesterId, TopicStatus status)
    {
        var queryable = GetQueryable();
        var topic = await queryable.Where(e => e.IsDeleted == false &&
                                                e.SemesterId == semesterId &&
                                                e.Status == status)
                                    .FirstOrDefaultAsync();
        return topic;
    }

    public async Task<Topic?> GetTopicOfUserIsPendingInSemester(Guid userId, Guid semesterId)
    {
        var queryable = GetQueryable();
        var topic = await queryable.Where(e => e.IsDeleted == false &&
                                                e.StageTopic != null &&
                                                e.StageTopic.Semester != null &&
                                                e.StageTopic.Semester.Id == semesterId &&
                                                (e.Status != TopicStatus.Draft && 
                                                    e.Status != TopicStatus.MentorRejected &&   
                                                    e.Status != TopicStatus.ManagerRejected))
                                    .FirstOrDefaultAsync();
        return topic;
    }
}