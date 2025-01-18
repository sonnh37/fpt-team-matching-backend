using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class Task : BaseEntity
{
    public Guid? ProjectId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? Priority { get; set; }

    public string? Status { get; set; }

    public DateTimeOffset? Deadline { get; set; }

    public Guid? AssignedToId { get; set; }

    public virtual User? AssignedTo { get; set; }

    public virtual Project? Project { get; set; }
}