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
            .Where(x => (x.UserId == null && x.Type == NotificationType.Individual) ||
                        (x.UserId != null && x.UserId == userId))
            .ToListAsync();

        return result ?? [];
    }
    
    public async Task<(List<Notification>, int)> GetDataByCurrentUser(
        NotificationGetAllByCurrentUserQuery query, 
        Guid userId, 
        Guid? projectId)
    {
        var queryable = GetQueryable();
        queryable = queryable.Include(m => m.User)
            .Include(m => m.Project)
            .Include(x => x.NotificationXUsers);
            

        // Điều kiện filter chính
        queryable = queryable.Where(x =>
                // Thông báo cá nhân (Individual)
                (x.Type == NotificationType.Individual && x.UserId == userId) ||

                // Thông báo hệ thống (SystemWide) - cho tất cả user
                (x.Type == NotificationType.SystemWide) ||
        
                // Thông báo nhóm (Team) - kiểm tra projectId
                (x.Type == NotificationType.Team && x.ProjectId == projectId) 
        
        );

        // Xử lý phân trang
        if (query.IsPagination)
        {
            var totalOrigin = await queryable.CountAsync();
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

    public async Task<List<Notification>> GetSystemNotification()
    {
        var result = await _dbContext.Notifications.Include(m => m.User)
            .Where(x => x.Type == NotificationType.SystemWide)
            .ToListAsync();
        return result;
    }

    public async Task<List<Notification>> GetTeamNotificationByProjectId(Guid projectId)
    {
        var result = await _dbContext.Notifications.Include(m => m.User)
            .Where(x => x.Type == NotificationType.Team && x.ProjectId == projectId).ToListAsync();
        return result;
    }
}