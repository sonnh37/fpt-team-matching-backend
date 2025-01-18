using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class PermissionXAction : BaseEntity
{
    public Guid? PermissionId { get; set; }

    public Guid? ActionId { get; set; }

    public bool Licensed { get; set; }

    public virtual Action? Action { get; set; }

    public virtual Permission? Permission { get; set; }
}