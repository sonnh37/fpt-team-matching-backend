using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class SemesterResult : BaseResult
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

    public CriteriaFormResult? CriteriaForm { get; set; }

    public ICollection<ProfileStudentResult> ProfileStudents { get; set; } = new List<ProfileStudentResult>();

    public ICollection<StageIdeaResult> StageIdeas { get; set; } = new List<StageIdeaResult>();

    public ICollection<TimelineResult> Timelines { get; set; } = new List<TimelineResult>();

    public ICollection<UserXRoleResult> UserXRoles { get; set; } = new List<UserXRoleResult>();

}