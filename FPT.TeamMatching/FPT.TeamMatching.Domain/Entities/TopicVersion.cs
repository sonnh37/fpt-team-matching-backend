using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Entities;

public class TopicVersion : BaseEntity
{
    public Guid? TopicId { get; set; }

    public string? FileUpdate { get; set; }
    
    public TopicVersionStatus? Status { get; set; }

    public string? Comment { get; set; }

    public int ReviewStage  { get; set; }

    public virtual Topic? Topic { get; set; }
}