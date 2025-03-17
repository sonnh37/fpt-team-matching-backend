using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaRequests;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;

namespace FPT.TeamMatching.Data.Repositories;

public class IdeaRequestRepository : BaseRepository<IdeaRequest>, IIdeaRequestRepository
{
    public IdeaRequestRepository(FPTMatchingDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<(List<IdeaRequest>, int)> GetData(IdeaRequestGetAllQuery query)
    {
        var queryable = GetQueryable();
        queryable = queryable.Include(m => m.Idea)
            .Include(m => m.Reviewer);

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


    public async Task<(List<IdeaRequest>, int)> GetIdeaRequestsCurrentByStatus(
        IdeaRequestGetAllCurrentByStatus query, Guid userId)
    {
        var status = (IdeaStatus)query.Status;
        var idea = await GetQueryable<Idea>()
            .OrderByDescending(m => m.CreatedDate)
            .Where(e => e.OwnerId == userId
                        && e.Status == status).FirstOrDefaultAsync();

        if (idea == null) return (new List<IdeaRequest>(), -1);

        var queryable = GetQueryable<IdeaRequest>()
            .Include(m => m.Idea)
            .Where(ir => ir.IdeaId == idea.Id && ir.Status == query.Status);

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

    public async Task<(List<IdeaRequest>, int)> GetCurrentIdeaRequestsByStatusAndRoles(
        IdeaRequestGetAllByListStatusForCurrentUser query, Guid userId)
    {
        var queryable = GetQueryable();
        queryable = queryable.Include(m => m.Idea)
            .Include(m => m.Reviewer);

        queryable = queryable.Where(m =>
            m.Status != null && m.Role != null &&
            (query.Roles.Contains(m.Role) && query.Status == m.Status && m.ReviewerId == userId));


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
    
    public async Task<(List<IdeaRequest>, int)> GetIdeaRequestsByStatusAndRoles(
        IdeaRequestGetAllByListStatusForCurrentUser query)
    {
        
        var queryable = GetQueryable();
        queryable = queryable.Include(m => m.Idea)
            .Include(m => m.Reviewer);

        queryable = queryable.Where(m =>
            m.Status != null && m.Role != null &&
            (query.Roles.Contains(m.Role) && query.Status == m.Status && query.IdeaId == m.IdeaId));


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

    public async Task<int> CountApprovedCouncilsForIdea(Guid ideaId)
    {
        return await GetQueryable()
            .Where(ir => ir.IdeaId == ideaId && ir.Role == "Council" && ir.Status == IdeaRequestStatus.Approved)
            .CountAsync();
    }
    
    public async Task<int> CountCouncilsForIdea(Guid ideaId)
    {
        return await GetQueryable()
            .Where(ir => ir.IdeaId == ideaId && ir.Role == "Council")
            .CountAsync();
    }
    
    public async Task<int> CountRejectedCouncilsForIdea(Guid ideaId)
    {
        return await GetQueryable()
            .Where(ir => ir.IdeaId == ideaId && ir.Role == "Council" && ir.Status == IdeaRequestStatus.Rejected)
            .CountAsync();
    }


    public async Task<(List<IdeaRequest>, int)> GetDataUnassignedReviewer(
        GetQueryableQuery query)
    {
        var queryable = GetQueryable();
        queryable = queryable.Include(m => m.Idea)
            .Include(m => m.Reviewer);
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