using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid? UserId { get; set; }

    public string? Description { get; set; }

    public NotificationType? Type { get; set; }
    
    public bool IsRead { get; set; }

    public virtual User? User { get; set; }
}