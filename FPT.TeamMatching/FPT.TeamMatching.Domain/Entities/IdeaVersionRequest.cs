using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Entities;

public class IdeaVersionRequest : BaseEntity
{
    public Guid? IdeaVersionId { get; set; }

    public Guid? ReviewerId { get; set; }

    public Guid? CriteriaFormId { get; set; }
    
    public IdeaVersionRequestStatus? Status { get; set; }
    
    public string? Role { get; set; }

    public DateTimeOffset? ProcessDate { get; set; }

    public virtual IdeaVersion? IdeaVersion { get; set; }

    public virtual User? Reviewer { get; set; }

    public virtual CriteriaForm? CriteriaForm { get; set; }

    public virtual ICollection<AnswerCriteria> AnswerCriterias { get; set; } = new List<AnswerCriteria>();
}