using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class Rate : BaseEntity
{
    public Guid? TeamMemberId { get; set; }

    public Guid? RateForId { get; set; }

    public Guid? RateById { get; set; }

    public int StarRating { get; set; }

    public virtual User? RateFor { get; set; }

    public virtual User? RateBy { get; set; }

    public virtual TeamMember? TeamMember { get; set; }
}