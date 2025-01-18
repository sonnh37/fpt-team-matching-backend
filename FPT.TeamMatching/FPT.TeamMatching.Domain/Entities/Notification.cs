using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid? UserId { get; set; }

    public string? Description { get; set; }

    public string? NotificationType { get; set; }

    public virtual User? User { get; set; }
}