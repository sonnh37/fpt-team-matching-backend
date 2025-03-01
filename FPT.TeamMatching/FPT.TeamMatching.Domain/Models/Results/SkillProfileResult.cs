using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class SkillProfileResult : BaseResult
{
    public Guid? ProfileStudentId { get; set; }

    public string? FullSkill { get; set; }

    public string? Json { get; set; }
    
    public ProfileStudentResult? ProfileStudent { get; set; }
}