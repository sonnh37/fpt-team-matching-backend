using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;

namespace FPT.TeamMatching.Data.Repositories
{
    public class NotificationXUserRepository : BaseRepository<NotificationXUser>, INotificationXUserRepository
    {
        public FPTMatchingDbContext _dbContext;
        public NotificationXUserRepository(FPTMatchingDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<NotificationXUser>> GetUnreadByUserId(Guid userId)
        {
            var response = await _dbContext.NotificationXUsers.Where(x => x.UserId == userId && x.IsRead == false).ToListAsync();
            return response;
        }

        public async Task<List<NotificationXUser>> GetAllByUserId(Guid userId)
        {
            var response = await _dbContext.NotificationXUsers.Where(x => x.UserId == userId).ToListAsync();
            return response;
        }

        public async Task<NotificationXUser> GetByUserIdAndNotiId(Guid userId, Guid notificationId)
        {
            var response = await _dbContext.NotificationXUsers.Where(x => x.UserId == userId && x.NotificationId == notificationId).FirstOrDefaultAsync();
            return response;
        }
    }
}
