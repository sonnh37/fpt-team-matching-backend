using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.SkillProfile;

public class SkillProfileCreateCommand : CreateCommand
{
    public Guid UserId { get; set; }

    public string? FullSkill { get; set; }

    public string? Json { get; set; }
}