using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class IdeaRequest : BaseEntity
{
    public Guid? IdeaId { get; set; }

    public Guid? ReviewerId { get; set; }
    
    public string? Content { get; set; }

    public DateTimeOffset? ProcessDate { get; set; }

    public virtual Idea? Idea { get; set; }

    public virtual User? Reviewer { get; set; }
}