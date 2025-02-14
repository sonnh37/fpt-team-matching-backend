using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class SkillProfile : BaseEntity
{
    public Guid? ProfileStudentId { get; set; }

    public string? FullSkill { get; set; }

    public string? Json { get; set; }

    public virtual ProfileStudent? ProfileStudent { get; set; }
}