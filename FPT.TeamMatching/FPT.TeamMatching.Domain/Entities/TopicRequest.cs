using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Entities;

public class TopicRequest : BaseEntity
{
    public Guid? TopicId { get; set; }

    public Guid? ReviewerId { get; set; }

    public Guid? CriteriaFormId { get; set; }
    
    public TopicRequestStatus? Status { get; set; }
    
    public DateTimeOffset? ProcessDate { get; set; }

    public string? Role { get; set; }

    public virtual Topic? Topic { get; set; }

    public virtual User? Reviewer { get; set; }

    public virtual CriteriaForm? CriteriaForm { get; set; }

    public virtual ICollection<AnswerCriteria> AnswerCriterias { get; set; } = new List<AnswerCriteria>();
}