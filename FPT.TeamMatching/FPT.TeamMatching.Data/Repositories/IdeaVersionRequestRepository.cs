using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaVersionRequest;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;

namespace FPT.TeamMatching.Data.Repositories;

public class IdeaVersionRequestRepository : BaseRepository<IdeaVersionRequest>, IIdeaVersionRequestRepository
{
    private readonly IIdeaRepository _ideaRepository;

    public IdeaVersionRequestRepository(FPTMatchingDbContext dbContext, IIdeaRepository ideaRepository) :
        base(dbContext)
    {
        _ideaRepository = ideaRepository;
    }

    public async Task<(List<IdeaVersionRequest>, int)> GetData(IdeaVersionRequestGetAllQuery query)
    {
        var queryable = GetQueryable();
        //sua db
        //queryable = queryable.Include(m => m.Idea)
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

    public async Task<(List<IdeaVersionRequest>, int)> GetDataExceptPending(IdeaVersionRequestGetAllQuery query)
    {
        var queryable = GetQueryable();
        //sua db
        //queryable = queryable.Include(m => m.Idea)
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


        queryable = queryable.Where(m => m.Status != IdeaVersionRequestStatus.Pending
              );
        

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

    public async Task<(List<IdeaVersionRequest>, int)> GetIdeaVersionRequestsForCurrentReviewerByRolesAndStatus(
        IdeaGetListByStatusAndRoleQuery query, Guid userId)
    {
        var queryable = GetQueryable();
        queryable = queryable
            .Include(m => m.IdeaVersion).ThenInclude(m => m.StageIdea)
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

    public async Task<int> CountApprovedCouncilsForIdea(Guid ideaId)
    {
        return await GetQueryable()
            //sua db
            .Where(ir => ir.IdeaVersion != null && ir.IdeaVersion.IdeaId == ideaId && ir.Role == "Council" &&
                         ir.Status == IdeaVersionRequestStatus.Approved)
            .CountAsync();
    }

    public async Task<int> CountCouncilsForIdea(Guid ideaId)
    {
        return await GetQueryable()
            //sua db
            .Where(ir => ir.IdeaVersion != null && ir.IdeaVersion.IdeaId == ideaId && ir.Role == "Council")
            .CountAsync();
    }

    public async Task<int> CountRejectedCouncilsForIdea(Guid ideaId)
    {
        return await GetQueryable()
            //sua db
            .Include(m => m.IdeaVersion)
            .Where(ir => ir.IdeaVersion != null && ir.IdeaVersion.IdeaId == ideaId && ir.Role == "Council" &&
                         ir.Status == IdeaVersionRequestStatus.Rejected)
            .CountAsync();
    }

    public async Task<int> CountConsiderCouncilsForIdea(Guid ideaId)
    {
        return await GetQueryable()
            //sua db
            .Include(m => m.IdeaVersion)
            .Where(ir => ir.IdeaVersion != null && ir.IdeaVersion.IdeaId == ideaId && ir.Role == "Council" &&
                         ir.Status == IdeaVersionRequestStatus.Consider)
            .CountAsync();
    }


    public async Task<(List<IdeaVersionRequest>, int)> GetDataUnassignedReviewer(
        GetQueryableQuery query)
    {
        var queryable = GetQueryable();
        //sua db
        //queryable = queryable.Include(m => m.Idea)
        //.Include(m => m.Reviewer);
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
}