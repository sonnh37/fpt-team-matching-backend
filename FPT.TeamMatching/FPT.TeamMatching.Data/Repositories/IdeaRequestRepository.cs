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


    public async Task<(List<IdeaRequest>, int)> GetDataByStatusAndIdeaId(
        IdeaRequestGetAllByListStatusAndIdeaIdQuery query)
    {
        var queryable = GetQueryable();
        queryable = queryable.Include(m => m.Idea)
            .Include(m => m.Reviewer);
        queryable = queryable.Where(m =>
            m.Status != null && (query.StatusList.Contains(m.Status.Value) && m.IdeaId == query.IdeaId));

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