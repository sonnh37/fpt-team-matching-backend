using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class ProjectActivity : BaseEntity
{
    public Guid? ProjectId { get; set; }

    public string? Content { get; set; }

    public virtual Project? Project { get; set; }
}