using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class SkillProfileResult : BaseResult
{
    public Guid? UserId { get; set; }

    public string? FullSkill { get; set; }

    public string? Json { get; set; }
}