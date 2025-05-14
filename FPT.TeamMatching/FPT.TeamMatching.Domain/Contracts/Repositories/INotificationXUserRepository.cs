using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Contracts.Repositories
{
    public interface INotificationXUserRepository: IBaseRepository<NotificationXUser>
    {
        Task<List<NotificationXUser>> GetUnreadByUserId(Guid userId);
        Task<List<NotificationXUser>> GetAllByUserId(Guid userId);
        Task<NotificationXUser> GetByUserIdAndNotiId(Guid userId, Guid notificationId);
    }
}
