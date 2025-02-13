using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FPT.TeamMatching.Data.Repositories;

public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
{
    private readonly FPTMatchingDbContext _dbContext;

    public NotificationRepository(FPTMatchingDbContext context, IMapper mapper) : base(context, mapper)
    {
        _dbContext = context;
    }

    public async Task<List<Notification>> GetAllNotificationByUserId(Guid userId)
    {
        var result = await _dbContext.Notifications.Where(x => x.UserId == userId)
            .ToListAsync();

        return result;
    }
}