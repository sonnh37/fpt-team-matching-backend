using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class Feedback : BaseEntity
{
    public Guid? ReviewId { get; set; }

    public string? Comment { get; set; }

    public virtual Review? Review { get; set; }
}