using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class SkillProfile : BaseEntity
{
    public Guid? UserId { get; set; }

    public string? FullSkill { get; set; }

    public string? Json { get; set; }

    public virtual User? User { get; set; }
}