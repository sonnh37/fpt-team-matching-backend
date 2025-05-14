using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class SemesterResult : BaseResult
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

    public CriteriaFormResult? CriteriaForm { get; set; }

    public virtual ICollection<ProfileStudentResult> ProfileStudents { get; set; } = new List<ProfileStudentResult>();

    public virtual ICollection<StageTopicResult> StageTopics { get; set; } = new List<StageTopicResult>();

    public virtual ICollection<UserXRoleResult> UserXRoles { get; set; } = new List<UserXRoleResult>();

    public virtual ICollection<ProjectResult> Projects { get; set; } = new List<ProjectResult>();

}