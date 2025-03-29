using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Entities;

public class IdeaHistory : BaseEntity
{
    public Guid? IdeaId { get; set; }

    public string? FileUpdate { get; set; }
    
    public IdeaHistoryStatus? Status { get; set; }

    public int ReviewStage  { get; set; }

    public virtual Idea? Idea { get; set; }

    //public virtual ICollection<IdeaHistoryRequest> IdeaHistoryRequests { get; set; } = new List<IdeaHistoryRequest>();
}