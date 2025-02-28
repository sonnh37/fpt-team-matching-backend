using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Notifications;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;

namespace FPT.TeamMatching.Data.Repositories;

public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
{
    private readonly FPTMatchingDbContext _dbContext;

    public NotificationRepository(FPTMatchingDbContext context) : base(context)
    {
        _dbContext = context;
    }

    public async Task<List<Notification>> GetAllNotificationByUserId(Guid userId)
    {
        var result = await _dbContext.Notifications.Include(m => m.User)
            .Where(x => (x.UserId == null && x.Type == NotificationType.General) ||
                        (x.UserId != null && x.UserId == userId))
            .ToListAsync();

        return result ?? [];
    }
    
    public async Task<(List<Notification>, int)> GetDataByCurrentUser(NotificationGetAllByCurrentUserQuery query, Guid userId)
    {
        var queryable = GetQueryable();
        
        // Filter and include
        queryable = queryable.Include(m => m.User)
            .Where(x => (x.UserId == null && x.Type == NotificationType.General) ||
                        (x.UserId != null && x.UserId == userId));
        // End
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