using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Requests.Queries.TopicRequest;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;

namespace FPT.TeamMatching.Data.Repositories;

public class TopicRequestRepository : BaseRepository<TopicRequest>, ITopicRequestRepository
{
    private readonly ITopicRepository _topicRepository;

    public TopicRequestRepository(FPTMatchingDbContext dbContext, ITopicRepository topicRepository) :
        base(dbContext)
    {
        _topicRepository = topicRepository;
    }


    public async Task<(List<TopicRequest>, int)> GetData(TopicRequestGetAllQuery query)
    {
        var queryable = GetQueryable();
        queryable = queryable
            .Include(m => m.Reviewer);
        queryable = queryable
            .Include(m => m.CriteriaForm);


        if (query.Status.HasValue)
        {
            queryable = queryable.Where(m =>
                m.Status != null && (query.Status == m.Status));
        }

        if (query.IsPagination)
        {
            // Tổng số count sau khi  filter khi chưa lọc trang
            var totalOrigin = queryable.Count();
            // Sắp sếp
            queryable = Sort(queryable, query);
            // Lọc trang
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

    public async Task<(List<TopicRequest>, int)> GetDataExceptPending(TopicRequestGetAllQuery query)
    {
        var queryable = GetQueryable();

        queryable = queryable
            .Include(m => m.Reviewer);
        queryable = queryable
       .Include(m => m.CriteriaForm);
        if (!string.IsNullOrEmpty(query.CreatedBy))
        {
            queryable = queryable.Where(m =>
                m.CreatedBy != null && m.CreatedBy.Contains(query.CreatedBy.Trim().ToLower()));
        }
        if (query.ReviewerId != null)
        {
            queryable = queryable.Where(m => m.ReviewerId == query.ReviewerId);
        }

        queryable = queryable.Where(m => m.Status != TopicRequestStatus.Pending);

        if (query.IsPagination)
        {
            // Tổng số count sau khi  filter khi chưa lọc trang
            var totalOrigin = queryable.Count();
            // Sắp sếp
            queryable = Sort(queryable, query);
            // Lọc trang
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

    public async Task<(List<TopicRequest>, int)> GetIdeaVersionRequestsForCurrentReviewerByRolesAndStatus(
        TopicRequestGetListByStatusAndRoleQuery query, Guid userId)
    {
        var queryable = GetQueryable();
        queryable = queryable
            .Include(m => m.Topic).ThenInclude(m => m.StageTopic)
            .Include(m => m.Reviewer);

        queryable = queryable.Where(m =>
            m.Status != null && m.Role != null &&
            query.Roles.Contains(m.Role) && query.Status == m.Status && m.ReviewerId == userId);

        queryable = Sort(queryable, query);

        var total = queryable.Count();
        var results = query.IsPagination
            ? await GetQueryablePagination(queryable, query).ToListAsync()
            : await queryable.ToListAsync();

        return (results, query.IsPagination ? total : results.Count);
    }

    //public async Task<int> CountStatusCouncilsForIdea(Guid topicId, TopicRequestStatus status)
    //{
    //    return await GetQueryable()
    //        .Where(ir => ir.IdeaVersion != null && ir.IdeaVersion.IdeaId == topicId && ir.Role == "Council" &&
    //                     ir.Status == status)
    //        .CountAsync();
    //}

    //public async Task<int> CountCouncilsForIdea(Guid topicId)
    //{
    //    return await GetQueryable()
    //        .Where(ir => ir.IdeaVersion != null && ir.IdeaVersion.IdeaId == topicId && ir.Role == "Council")
    //        .CountAsync();
    //}

    public async Task<(List<TopicRequest>, int)> GetDataUnassignedReviewer(
        GetQueryableQuery query)
    {
        var queryable = GetQueryable();

        queryable = queryable.Where(m => m.ReviewerId == null);

        if (query.IsPagination)
        {
            // Tổng số count sau khi  filter khi chưa lọc trang
            var totalOrigin = queryable.Count();
            // Sắp sếp
            queryable = Sort(queryable, query);
            // Lọc trang
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

    public async Task<List<TopicRequest>?> GetRoleMentorNotApproveInSemester(Guid semesterId)
    {
        var queryable = GetQueryable();

        var topicVersionRequests = await queryable.Where(e => e.IsDeleted == false &&
                                                             e.Role == "Mentor" &&
                                                             e.Status != TopicRequestStatus.Approved &&
                                                             e.Topic != null &&
                                                             e.Topic.StageTopic != null &&
                                                             e.Topic.StageTopic.Semester != null &&
                                                             e.Topic.StageTopic.Semester.Id == semesterId)
            .ToListAsync();

        return topicVersionRequests;
    }

    public Task<int> CountStatusCouncilsForIdea(Guid topicId, TopicRequestStatus status)
    {
        throw new NotImplementedException();
    }

    public Task<(List<TopicRequest>, int)> GetTopicVersionRequestsForCurrentReviewerByRolesAndStatus(TopicRequestGetListByStatusAndRoleQuery query, Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountCouncilsForIdea(Guid ideaId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<TopicRequest>> GetByTopicIdAndRoleAndStatus(Guid topicId, string role, TopicRequestStatus status)
    {
        var queryable = GetQueryable();

        var topicRequests = await queryable.Where(e => e.IsDeleted == false &&
                                                       e.TopicId == topicId &&
                                                       e.Role == role &&
                                                       e.Status == status)
                                            .ToListAsync();

        return topicRequests;
    }
    
    public async Task<List<TopicRequest>> GetByTopicIdAndRole(Guid topicId, string role)
    {
        var queryable = GetQueryable();

        var topicRequests = await queryable.Where(e => e.IsDeleted == false &&
                                                       e.TopicId == topicId &&
                                                       e.Role == role)
            .ToListAsync();

        return topicRequests;
    }
}