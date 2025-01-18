using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class UserXPermission : BaseEntity
{
    public Guid? PermissionId { get; set; }

    public Guid? UserId { get; set; }

    public Guid Licensed { get; set; }

    public virtual Permission? Permission { get; set; }

    public virtual User? User { get; set; }
}