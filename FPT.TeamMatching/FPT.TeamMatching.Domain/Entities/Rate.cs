using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class Rate : BaseEntity
{

    public Guid? RateForId { get; set; }

    public Guid? RateById { get; set; }

    public int NumbOfStar { get; set; }

    public string? Content { get; set; }

    public virtual TeamMember? RateFor { get; set; }

    public virtual TeamMember? RateBy { get; set; }
}