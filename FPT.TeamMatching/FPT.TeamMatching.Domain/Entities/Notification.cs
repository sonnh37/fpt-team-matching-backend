using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid? ProjectId { get; set; }

    public Guid? UserId { get; set; }

    public string? Description { get; set; }

    public NotificationType? Type { get; set; }

    public virtual User? User { get; set; }

    public virtual Project? Project { get; set; }

    public virtual ICollection<NotificationXUser> NotificationXUsers { get; set; } = new List<NotificationXUser>();
}