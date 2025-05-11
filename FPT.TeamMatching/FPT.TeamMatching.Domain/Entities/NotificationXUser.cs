using FPT.TeamMatching.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Entities
{
    public class NotificationXUser: BaseEntity
    {
        public Guid? NotificationId { get; set; }

        public Guid? UserId { get; set; }

        public bool IsRead { get; set; }

        public virtual Notification? Notification { get; set; }

        public virtual User? User { get; set; }
    }
}
