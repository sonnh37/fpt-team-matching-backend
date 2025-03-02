using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class Semester : BaseEntity
{
    public string? SemesterCode { get; set; }

    public string? SemesterName { get; set; }

    public DateTimeOffset? StartDate { get; set; }

    public DateTimeOffset? EndDate { get; set; }

    public virtual ICollection<Idea> Ideas { get; set; } = new List<Idea>();
}