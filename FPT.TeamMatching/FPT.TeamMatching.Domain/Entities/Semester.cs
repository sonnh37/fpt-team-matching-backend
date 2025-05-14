using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Entities;

public class Semester : BaseEntity
{
    public Guid? CriteriaFormId { get; set; }

    public string? SemesterCode { get; set; }

    public string? SemesterName { get; set; }

    public string? SemesterPrefixName { get; set; }

    public DateTimeOffset? StartDate { get; set; }

    public DateTimeOffset? EndDate { get; set; }

    public DateTimeOffset? OnGoingDate { get; set; }

    public DateTimeOffset? PublicTopicDate { get; set; }

    public SemesterStatus Status { get; set; }

    public int MaxTeamSize { get; set; }

    public int MinTeamSize { get; set; }

    public int NumberOfTeam { get; set; }

    public int LimitTopicMentorOnly { get; set; }

    public int LimitTopicSubMentor { get; set; }

    public CriteriaForm? CriteriaForm { get; set; }

    public virtual ICollection<ProfileStudent> ProfileStudents { get; set; } = new List<ProfileStudent>();
    
    public virtual ICollection<StageTopic> StageTopics { get; set; } = new List<StageTopic>();

    public virtual ICollection<UserXRole> UserXRoles { get; set; } = new List<UserXRole>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

}