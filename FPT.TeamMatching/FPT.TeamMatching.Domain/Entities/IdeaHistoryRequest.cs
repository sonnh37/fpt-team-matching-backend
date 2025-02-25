using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Entities;

public class IdeaHistoryRequest : BaseEntity
{
    public Guid? IdeaHistoryId { get; set; }

    public Guid? ReviewerId { get; set; }
    
    public string? Content { get; set; }
    
    public IdeaHistoryRequestStatus? Status { get; set; }

    public DateTimeOffset? ProcessDate { get; set; }

    public virtual IdeaHistory? IdeaHistory { get; set; }

    public virtual User? Reviewer { get; set; }
}