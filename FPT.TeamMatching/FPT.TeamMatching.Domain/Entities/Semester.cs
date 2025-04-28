using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class Semester : BaseEntity
{
    public Guid? CriteriaFormId { get; set; }

    public string? SemesterCode { get; set; }

    public string? SemesterName { get; set; }

    public string? SemesterPrefixName { get; set; }

    public DateTimeOffset? PublicTopicDate { get; set; }

    public DateTimeOffset? StartDate { get; set; }

    public DateTimeOffset? EndDate { get; set; }

    public int LimitTopicMentorOnly { get; set; }

    public int LimitTopicSubMentor { get; set; }

    public CriteriaForm? CriteriaForm { get; set; }

    public virtual ICollection<ProfileStudent> ProfileStudents { get; set; } = new List<ProfileStudent>();
    
    public virtual ICollection<StageIdea> StageIdeas { get; set; } = new List<StageIdea>();

    public virtual ICollection<Timeline> Timelines { get; set; } = new List<Timeline>();
    
    public virtual ICollection<UserXRole> UserXRoles { get; set; } = new List<UserXRole>();

}